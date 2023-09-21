import { BaseRenderer } from "./baseRenderer";
import { Vec2 } from "./primitves/vec2";
import { fastline_shader } from "./shaders";

export class FastLineRenderer extends BaseRenderer {

    protected getShader(): GPUShaderModuleDescriptor {
        return fastline_shader;
    }

    protected getTopology(): GPUPrimitiveTopology {
        return "triangle-strip" as const;
    }

    protected getVertexBufferLayout(): GPUVertexBufferLayout {
        return {
            arrayStride: 40,
            attributes: [{
                format: "float32x2",
                offset: 0,
                shaderLocation: 0,
            },
            {
                format: "float32x2",
                offset: 8,
                shaderLocation: 1,
            },
            {
                format: "float32x2",
                offset: 16,
                shaderLocation: 2,
            },
            {
                format: "float32x4",
                offset: 24,
                shaderLocation: 3,
            }],

        };
    }

    protected override createVertices() {
        let data = [];
        for (let i = 0; i < this.signals.length; i++) {
            let samples = this.signals[i].samples;
            let color = this.signals[i].color;
            this.signals[i].gpuSampleCount = 2 * samples.length;
            data.push(samples[0]);
            data.push(samples[0]);
            data.push(samples[1]);
            addColorDir(color, 1);
            data.push(samples[0]);
            data.push(samples[0]);
            data.push(samples[1]);
            addColorDir(color, -1);
            for (let j = 1; j < samples.length - 1; j++) {
                data.push(samples[j - 1]);
                data.push(samples[j]);
                data.push(samples[j + 1]);
                addColorDir(color, 1);
                data.push(samples[j - 1]);
                data.push(samples[j]);
                data.push(samples[j + 1]);
                addColorDir(color, -1);
            }
            data.push(samples[samples.length - 2]);
            data.push(samples[samples.length - 1]);
            data.push(samples[samples.length - 1]);
            addColorDir(color, 1);
            data.push(samples[samples.length - 2]);
            data.push(samples[samples.length - 1]);
            data.push(samples[samples.length - 1]);
            addColorDir(color, -1);
        }
        this.vertices = new Float32Array(data.flatMap(x => [x.x, x.y]));

        this.vertexBuffer = this.device.createBuffer({
            label: "vertex buffer",
            size: this.vertices.byteLength,
            usage: GPUBufferUsage.VERTEX | GPUBufferUsage.COPY_DST,
        });

        this.device.queue.writeBuffer(this.vertexBuffer, 0, this.vertices);

        // add color and direction encoding
        function addColorDir(color: Float32Array, sign: number) {
            if (color.length != 4)
                throw new Error("Wrong Color Format");
            data.push(new Vec2(color[0], color[1]));
            data.push(new Vec2(color[2], sign * color[3]));
        }
    }

    override render() {
        this.updateUniforms();
        this.device.queue.writeBuffer(this.transformBuffer, 0, this.viewTransform);
        this.device.queue.writeBuffer(this.screenBuffer, 0, new Float32Array([this.canvas.width, this.canvas.height]));
        this.device.queue.writeBuffer(this.colorBuffer, 0, this.signals[0].color);
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
        for (let i = 0; i < this.signals.length; i++) {
            renderPass.draw(this.signals[i].gpuSampleCount, 1, i * this.signals[i].gpuSampleCount, 0);
        }
        renderPass.end();
        this.device.queue.submit([commandEncoder.finish()]);
    }
}