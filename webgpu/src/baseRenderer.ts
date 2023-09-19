import { Span } from "./primitves/span";
import { Vec2 } from "./primitves/vec2";
import { Signal } from "./signal";

export abstract class BaseRenderer {

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

    xRange: Span = new Span(-1, 1);
    yRange: Span = new Span(-1, 1);;

    protected viewTransform!: Float32Array;
    protected device!: GPUDevice;
    protected context!: GPUCanvasContext;
    protected canvasFormat!: GPUTextureFormat;

    protected colorBuffer!: GPUBuffer;
    protected transformBuffer!: GPUBuffer;
    protected screenBuffer!: GPUBuffer;
    protected vertexBuffer!: GPUBuffer;
    protected vertices!: Float32Array;
    protected pipeline!: GPURenderPipeline;
    protected bindingGroup!: GPUBindGroup;

    constructor(protected canvas: HTMLCanvasElement, protected signals: Signal[]) { }

    protected abstract getShader(): GPUShaderModuleDescriptor;
    protected abstract getVertexBufferLayout(): GPUVertexBufferLayout;
    protected abstract getTopology(): GPUPrimitiveTopology;
    protected abstract createVertices(): void;

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
        });

        // create default buffers
        this.colorBuffer = this.device.createBuffer({
            label: "color buffer",
            size: 16,
            usage: GPUBufferUsage.UNIFORM | GPUBufferUsage.COPY_DST
        });

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

        // create pipeline
        let shaderModule = this.device.createShaderModule(this.getShader())
        let vertexBufferLayout = this.getVertexBufferLayout();
        this.pipeline = await this.device.createRenderPipelineAsync({
            label: "plot pipeline",
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
                    format: this.canvasFormat
                }]
            },
            primitive: {
                topology: this.getTopology(),
            },
        });

        this.bindingGroup = this.device.createBindGroup({
            label: "binding group",
            layout: this.pipeline.getBindGroupLayout(0),
            entries: [{
                binding: 0,
                resource: { buffer: this.colorBuffer }
            },
            {
                binding: 1,
                resource: { buffer: this.transformBuffer }
            },
            {
                binding: 2,
                resource: { buffer: this.screenBuffer }
            }
            ]
        });

        // write vertex data to buffer
        this.createVertices();
    }

    render() {
        this.updateUniforms();
        this.device.queue.writeBuffer(this.transformBuffer, 0, this.viewTransform);
        this.device.queue.writeBuffer(this.screenBuffer, 0, new Float32Array([this.canvas.width, this.canvas.height]));
        for (let i = 0; i < this.signals.length; i++) {
            this.device.queue.writeBuffer(this.colorBuffer, 0, this.signals[i].color);

            const commandEncoder = this.device.createCommandEncoder();
            const renderPassDescriptor = {
                colorAttachments: [
                    {
                        view: this.context.getCurrentTexture().createView(),
                        loadOp: "load" as const,
                        clearValue: { r: 0, g: 0, b: 0.4, a: 1.0 },
                        storeOp: "store" as const,
                    },
                ],
            };

            const renderPass = commandEncoder.beginRenderPass(renderPassDescriptor);
            renderPass.setPipeline(this.pipeline);
            renderPass.setBindGroup(0, this.bindingGroup);
            renderPass.setVertexBuffer(0, this.vertexBuffer);
            renderPass.draw(this.signals[i].gpuSampleCount, 1, i * this.signals[i].gpuSampleCount, 0);
            renderPass.end();
            this.device.queue.submit([commandEncoder.finish()]);
        }
    }

    private updateUniforms() {
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
    }
}