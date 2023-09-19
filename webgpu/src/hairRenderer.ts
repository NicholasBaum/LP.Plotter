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
            arrayStride: 8,
            attributes: [{
                format: "float32x2",
                offset: 0,
                shaderLocation: 0,
            },],
        };
    }

    protected override createVertices() {
        let data: Vec2[] = [];
        for (let i = 0; i < this.signals.length; i++) {
            let source = this.signals[i].samples;
            this.signals[i].gpuSampleCount = source.length;
            data.push(...source);
        }
        this.vertices = new Float32Array(data.flatMap(x => [x.x, x.y]));
        this.vertexBuffer = this.device.createBuffer({
            label: "vertex buffer",
            size: this.vertices.byteLength,
            usage: GPUBufferUsage.VERTEX | GPUBufferUsage.COPY_DST,
        });

        this.device.queue.writeBuffer(this.vertexBuffer, 0, this.vertices);
    }
}