import { Color } from "./primitves/colors";
import { Vec2 } from "./primitves/vec2";

export class Signal {
    vertexCount: number = 0;
    constructor(public samples: Vec2[], public color: Color, public readonly thickness: number = 1.0) { }
    get gpuData(): SignalAttributes {
        return new SignalAttributes(this.color, this.thickness);
    }
}


export class SignalAttributes {

    static get byteLength(): number {
        return 8 * 4;
    }

    toFloats32(): number[] {
        // padding because size has to be a multiple of 16 
        return [...this.color.toFloats32(), ...[this.thickness, 0.0, 0.0, 0.0]];
    }

    constructor(public color: Color, public thickness: number) {

    }
}