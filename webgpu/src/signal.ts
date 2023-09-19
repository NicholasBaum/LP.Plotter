import { Vec2 } from "./primitves/vec2";

export class Signal {
    gpuSampleCount: number = 0;
    constructor(public samples: Vec2[], public color: Float32Array) { }
}