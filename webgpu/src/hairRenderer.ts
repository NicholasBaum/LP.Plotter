import { BaseRenderer } from "./baseRenderer";
import { Vec2 } from "./primitves/vec2";
import { hair_shader } from "./shaders";

export class HairRenderer extends BaseRenderer {

    protected getShader(): GPUShaderModuleDescriptor {
        return hair_shader;
    }

    protected getTopology(): GPUPrimitiveTopology {
        return "line-strip" as const;
    }

    protected getVertexBufferLayout(): GPUVertexBufferLayout {
        return {
            arrayStride: 24,
            attributes: [{
                format: "float32x2",
                offset: 0,
                shaderLocation: 0,
            },
            {
                format: "float32x4",
                offset: 8,
                shaderLocation: 1,
            },],
        };
    }

    protected override createVertices() {
        let data: Vec2[] = [];
        for (let i = 0; i < this.signals.length; i++) {
            let samples = this.signals[i].samples;
            this.signals[i].gpuSampleCount = samples.length;
            let color = this.signals[i].color;
            for (let j = 0; j < samples.length; j++) {
                data.push(samples[j]);
                data.push(new Vec2(color[0], color[1]));
                data.push(new Vec2(color[2], color[3]));
            }
        }
        let vertices = new Float32Array(data.flatMap(x => [x.x, x.y]));
        this.vertexBuffer = this.device.createBuffer({
            label: "vertex buffer",
            size: vertices.byteLength,
            usage: GPUBufferUsage.VERTEX | GPUBufferUsage.COPY_DST,
        });

        this.device.queue.writeBuffer(this.vertexBuffer, 0, vertices);
    }
}