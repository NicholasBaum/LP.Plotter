import { Vec2 } from "./primitves/vec2";

export class Signal {
    vertexCount: number = 0;
    constructor(public samples: Vec2[], public color: Float32Array, public readonly thickness: number = 1.0) { }
    get gpuData(): SignalAttributes {
        return new SignalAttributes(this.color, this.thickness);
    }
}


export class SignalAttributes {

    static get byteLength(): number {
        return 8 * 4;
    }

    toFloats32(): number[] {
        return [this.color[0], this.color[1], this.color[2], this.color[3], this.thickness, 0.0, 0.0, 0.0];
    }
    constructor(public color: Float32Array, public thickness: number) {

    }
}