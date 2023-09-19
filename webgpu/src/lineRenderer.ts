import { BaseRenderer } from "./baseRenderer";
import { Vec2 } from "./primitves/vec2";
import { miter_shader } from "./shaders";

export class LineRenderer extends BaseRenderer {

    protected getShader(): GPUShaderModuleDescriptor {
        return miter_shader;
    }

    protected getTopology(): GPUPrimitiveTopology {
        return "triangle-strip" as const;
    }

    protected getVertexBufferLayout(): GPUVertexBufferLayout {
        return {
            arrayStride: 32,
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
                format: "float32x2",
                offset: 24,
                shaderLocation: 3,
            }],

        };
    }

    protected override createVertices() {
        let data = [];
        for (let i = 0; i < this.signals.length; i++) {
            let source = this.signals[i].samples;
            this.signals[i].gpuSampleCount = 2 * source.length;
            data.push(source[0]);
            data.push(source[0]);
            data.push(source[1]);
            data.push(new Vec2(1, 1));
            data.push(source[0]);
            data.push(source[0]);
            data.push(source[1]);
            data.push(new Vec2(-1, -1));
            for (let j = 1; j < source.length - 1; j++) {
                data.push(source[j - 1]);
                data.push(source[j]);
                data.push(source[j + 1]);
                data.push(new Vec2(1, 1));
                data.push(source[j - 1]);
                data.push(source[j]);
                data.push(source[j + 1]);
                data.push(new Vec2(-1, -1));
            }
            data.push(source[source.length - 2]);
            data.push(source[source.length - 1]);
            data.push(source[source.length - 1]);
            data.push(new Vec2(1, 1));
            data.push(source[source.length - 2]);
            data.push(source[source.length - 1]);
            data.push(source[source.length - 1]);
            data.push(new Vec2(-1, -1));
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