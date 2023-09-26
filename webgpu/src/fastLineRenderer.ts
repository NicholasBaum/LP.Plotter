import { BaseRenderer } from "./baseRenderer";
import { Vec2 } from "./primitves/vec2";
import { fastline_shader } from "./shaders/fastline_shader";
import { Signal } from "./signal";

export enum AAModes {
    None,
    Local,
    MSAA4
}
export class FastLineRenderer extends BaseRenderer {

    constructor(canvas: HTMLCanvasElement, signals: Signal[], public aAMode: AAModes = AAModes.None) {
        super(canvas, signals);

    }
    protected getShader(): GPUShaderModuleDescriptor {
        switch (this.aAMode) {
            case AAModes.None:
                this.useMSAA = false;
                break;
            case AAModes.Local:
                this.useMSAA = false;
                break;
            case AAModes.MSAA4:
                this.useMSAA = true;
                break;
        }

        return fastline_shader(this.aAMode == AAModes.Local);
    }

    protected getTopology(): GPUPrimitiveTopology {
        return "triangle-strip" as const;
    }

    protected getVertexBufferLayout(): GPUVertexBufferLayout {
        return {
            arrayStride: 32,
            // prev vertex
            attributes: [{
                format: "float32x2",
                offset: 0,
                shaderLocation: 0,
            },
            // current vertex
            {
                format: "float32x2",
                offset: 8,
                shaderLocation: 1,
            },
            // next vertex
            {
                format: "float32x2",
                offset: 16,
                shaderLocation: 2,
            },
            // direction to offset and signal id
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
            let samples = this.signals[i].samples;
            this.signals[i].vertexCount = 4 * samples.length - 4;

            for (let vert = 1; vert <= 2; vert++) {
                data.push(samples[0]);
                data.push(samples[0]);
                data.push(samples[1]);
                addColorDir(i, vert);
            }

            for (let j = 1; j < samples.length - 1; j++) {
                for (let vert = 1; vert <= 4; vert++) {
                    data.push(samples[j - 1]);
                    data.push(samples[j]);
                    data.push(samples[j + 1]);
                    addColorDir(i, vert);
                }
            }

            for (let vert = 1; vert <= 2; vert++) {
                data.push(samples[samples.length - 2]);
                data.push(samples[samples.length - 1]);
                data.push(samples[samples.length - 1]);
                addColorDir(i, vert);
            }
        }
        let vertices = new Float32Array(data.length * 2);

        for (let i = 0; i < data.length; i++) {
            vertices[2 * i] = data[i].x;
            vertices[2 * i + 1] = data[i].y;
        }

        this.vertexBuffer = this.device.createBuffer({
            label: "vertex buffer",
            size: vertices.byteLength,
            usage: GPUBufferUsage.VERTEX | GPUBufferUsage.COPY_DST,
        });

        this.device.queue.writeBuffer(this.vertexBuffer, 0, vertices);

        // add color and direction encoding
        function addColorDir(index: number, sign: number) {

            data.push(new Vec2(index, sign));
        }
    }
}