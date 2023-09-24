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
            arrayStride: 16,
            attributes: [{
                format: "float32x2",
                offset: 0,
                shaderLocation: 0,
            },
            {
                format: "float32x2",
                offset: 8,
                shaderLocation: 1,
            },],
        };
    }

    protected override createVertices() {
        let tmp: Vec2[] = [];
        for (let i = 0; i < this.signals.length; i++) {
            let signal = this.signals[i];            
            signal.vertexCount = signal.samples.length;
            for (let j = 0; j < signal.samples.length; j++) {
                tmp.push(signal.samples[j]);
                tmp.push(new Vec2(i, 0));
            }
        }
        let data : number[] =[];
        for(let i =0; i<tmp.length;i++)
        {
             data.push(...tmp[i].toFloats32())            
        }
        let vertices = new Float32Array(data);
        this.vertexBuffer = this.device.createBuffer({
            label: "vertex buffer",
            size: vertices.byteLength,
            usage: GPUBufferUsage.VERTEX | GPUBufferUsage.COPY_DST,
        });

        this.device.queue.writeBuffer(this.vertexBuffer, 0, vertices);
    }
}