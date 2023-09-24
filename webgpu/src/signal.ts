import { Color } from "./primitves/colors";
import { Span } from "./primitves/span";
import { Vec2 } from "./primitves/vec2";

export class Signal {
    vertexCount: number = 0;
    constructor(public samples: Vec2[], public color: Color, public readonly thickness: number = 1.0, public yViewRange: Span = new Span(-1, 1)) { }
    get gpuData(): SignalAttributes {
        return new SignalAttributes(this.color, this.thickness, this.yViewRange);
    }
}


export class SignalAttributes {

    static get byteLength(): number {
        return 8 * 4 + 16 * 4;
    }

    toFloats32(): number[] {
        // padding because size has to be a multiple of 16 
        return [...this.color.toFloats32(), ...[this.thickness, 0.0, 0.0, 0.0], ...this.getTransforms()];
    }

    constructor(public readonly color: Color, public readonly thickness: number, public readonly yViewRange: Span) {

    }

    private getTransforms(): Float32Array {
        let sy = 2 / (this.yViewRange.max - this.yViewRange.min);
        let m11 = sy;
        let m31 = -sy * this.yViewRange.min - 1;
        let viewTransform = new Float32Array(
            [1, 0, 0, 0,
                0, m11, 0, 0,
                0, 0, 1, 0,
                0, m31, 0, 1,]);
        return viewTransform;
    }
}