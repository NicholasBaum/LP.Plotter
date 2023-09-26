import { Span } from "./primitves/span";
import { Vec2 } from "./primitves/vec2";
import { Signal, SignalAttributes } from "./signal";

export abstract class BaseRenderer {

    xRange: Span = new Span(-1, 1);
    // should be the default value if you want to use different yranges per line
    // because currently transforms just get multiplied in the shader
    yRange: Span = new Span(-1, 1);

    public useMSAA = false;
    private readonly aaSampleCount = 4; // other values aren't allowed i think

    protected viewTransform!: Float32Array;
    protected device!: GPUDevice;
    protected context!: GPUCanvasContext;
    protected canvasFormat!: GPUTextureFormat;

    protected transformBuffer!: GPUBuffer;
    protected screenBuffer!: GPUBuffer;
    protected signalsAttrBuffer!: GPUBuffer;
    protected vertexBuffer!: GPUBuffer;
    protected pipeline!: GPURenderPipeline;
    protected bindingGroup!: GPUBindGroup;

    private lastCanvasSize: Vec2 = new Vec2(0, 0);
    
    constructor(protected canvas: HTMLCanvasElement, protected signals: Signal[]) { }

    protected abstract getShader(): GPUShaderModuleDescriptor;
    protected abstract getVertexBufferLayout(): GPUVertexBufferLayout;
    protected abstract getTopology(): GPUPrimitiveTopology;
    protected abstract createVertices(): void;

    zoom(factor: number, pos: Vec2) {
        let x = pos.x / this.canvas.width;
        this.xRange.zoomRelative(factor, x);
        let y = 1 - pos.y / this.canvas.height;
        this.yRange.zoomRelative(factor, y);
    }

    pan(pan: Vec2) {
        let panx = pan.x / this.canvas.width;
        this.xRange.panRelative(panx);
        let pany = pan.y / this.canvas.height;
        this.yRange.panRelative(pany);
    }

    protected createBindingGroup(): GPUBindGroupDescriptor {
        return {
            label: "binding group",
            layout: this.pipeline.getBindGroupLayout(0),
            entries: [{
                binding: 0,
                resource: { buffer: this.transformBuffer }
            },
            {
                binding: 1,
                resource: { buffer: this.screenBuffer }
            },
            {
                binding: 2,
                resource: { buffer: this.signalsAttrBuffer }
            },
            ]
        }
    }

    protected createBuffers() {
        this.transformBuffer = this.device.createBuffer({
            label: "transform buffer",
            size: 64,
            usage: GPUBufferUsage.UNIFORM | GPUBufferUsage.COPY_DST
        });

        this.screenBuffer = this.device.createBuffer({
            label: "screen size buffer",
            size: 16,
            usage: GPUBufferUsage.UNIFORM | GPUBufferUsage.COPY_DST
        });

        this.signalsAttrBuffer = this.device.createBuffer({
            label: "signal attributes buffer",
            size: this.signals.length * SignalAttributes.byteLength,
            usage: GPUBufferUsage.STORAGE | GPUBufferUsage.COPY_DST
        });
    }

    async initialize() {
        // WebGPU device initialization
        if (!navigator.gpu) {
            throw new Error("WebGPU not supported on this browser.");
        }

        const adapter = await navigator.gpu.requestAdapter();
        if (!adapter) {
            throw new Error("No appropriate GPUAdapter found.");
        }

        this.device = await adapter.requestDevice();

        // Canvas configuration
        this.context = <unknown>this.canvas.getContext("webgpu") as GPUCanvasContext;
        this.canvasFormat = navigator.gpu.getPreferredCanvasFormat();
        this.context.configure({
            device: this.device,
            format: this.canvasFormat,
            //alphaMode: 'premultiplied',
        });

        this.createBuffers();

        // create pipeline
        let shaderModule = this.device.createShaderModule(this.getShader())
        let vertexBufferLayout = this.getVertexBufferLayout();
        this.pipeline = await this.device.createRenderPipelineAsync({
            label: "core pipeline",
            layout: "auto",
            vertex: {
                module: shaderModule,
                entryPoint: "vertexMain",
                buffers: [vertexBufferLayout]
            },
            fragment: {
                module: shaderModule,
                entryPoint: "fragmentMain",
                targets: [{
                    format: this.canvasFormat,
                    blend: {
                        color: { srcFactor: "src-alpha", dstFactor: "one-minus-src-alpha", operation: "add" },
                        alpha: {}
                    }
                }]
            },
            primitive: {
                topology: this.getTopology(),
            },
            multisample: this.useMSAA ?
                { count: this.aaSampleCount, }
                : undefined,
        });

        this.bindingGroup = this.device.createBindGroup(this.createBindingGroup());
        // write vertex data to buffer
        this.createVertices();
        this.writeSignalAttributes();
    }

    render() {
        this.updateTransforms();
        this.updateScreenSizeIfChanged();
        const commandEncoder = this.device.createCommandEncoder();

        let view: GPUTextureView;
        if (this.useMSAA) {
            const texture = this.device.createTexture({
                size: [this.canvas.width, this.canvas.height],
                sampleCount: this.aaSampleCount,
                format: this.canvasFormat,
                usage: GPUTextureUsage.RENDER_ATTACHMENT,
            });
            view = texture.createView();
        }
        else {
            view = this.context.getCurrentTexture().createView();
        }

        const renderPassDescriptor = {
            colorAttachments: [
                {
                    view: view,
                    resolveTarget: this.useMSAA ? this.context.getCurrentTexture().createView() : undefined,
                    loadOp: "clear" as const,
                    clearValue: { r: 0, g: 0.0, b: 0.0, a: 1.0 },
                    storeOp: "store" as const,
                },
            ],
        };
        const renderPass = commandEncoder.beginRenderPass(renderPassDescriptor);
        renderPass.setPipeline(this.pipeline);
        renderPass.setBindGroup(0, this.bindingGroup);
        renderPass.setVertexBuffer(0, this.vertexBuffer);
        for (let i = 0; i < this.signals.length; i++) {
            renderPass.draw(this.signals[i].vertexCount, 1, i * this.signals[i].vertexCount, 0);
        }
        renderPass.end();
        this.device.queue.submit([commandEncoder.finish()]);
    }


    private writeSignalAttributes() {
        this.device.queue.writeBuffer(this.signalsAttrBuffer, 0, new Float32Array(this.signals.flatMap(s => s.gpuData.toFloats32())));
    }

    protected updateTransforms() {
        let sx = 2 / (this.xRange.max - this.xRange.min);
        let m00 = sx;
        let m30 = -sx * this.xRange.min - 1;
        let sy = 2 / (this.yRange.max - this.yRange.min);
        let m11 = sy;
        let m31 = -sy * this.yRange.min - 1;
        this.viewTransform = new Float32Array(
            [m00, 0, 0, 0,
                0, m11, 0, 0,
                0, 0, 1, 0,
                m30, m31, 0, 1,]);
        this.device.queue.writeBuffer(this.transformBuffer, 0, this.viewTransform);
    }

    private updateScreenSizeIfChanged() {
        let current = new Vec2(this.canvas.width, this.canvas.height);
        if (!this.lastCanvasSize.equals(current)) {
            this.lastCanvasSize = current;
            this.device.queue.writeBuffer(this.screenBuffer, 0, new Float32Array([this.canvas.width, this.canvas.height]));
        }
    }
}