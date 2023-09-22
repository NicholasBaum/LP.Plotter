import { colors } from "./primitves/colors";
import { Span } from "./primitves/span";
import { Vec2 } from "./primitves/vec2";
import seedrandom from "./seedrandom";
import { Signal } from "./signal";

export class DataGenerator {

    static rng = seedrandom(1337);

    static create(): [signals: Signal[], xRange: Span, yRange: Span] {
        let xRange = new Span(-1, 1);
        let yRange = new Span(-1, 1);
        let signals: Signal[] = [];

        let points = [
            new Vec2(-0.8, -0.6),
            new Vec2(-0.8, 0.6),
            new Vec2(-0.4, 0.6),
            new Vec2(-0.2, 0.6),
            new Vec2(0.0, 0.6),
            new Vec2(0.4, -0.3),
            new Vec2(0.8, 0.1),
        ];
        signals[0] = new Signal(points, colors[0]);
        return [signals, xRange, yRange];
    }

    static createMultiLineData(seriesCount = 500, sampleCount = 500): [signals: Signal[], xRange: Span, yRange: Span] {
        let xRange = new Span(0, sampleCount);
        let signals: Signal[] = [];
        let yMax = 0;
        let yMin = 0;
        for (var i = 0; i < seriesCount; i++) {
            let points = [];
            let prev = 0;
            for (var j = 0; j < sampleCount; j++) {
                let curr = DataGenerator.rng() * 10 - 5;
                points.push(new Vec2(j, prev + curr));
                prev += curr;
                if (prev > yMax)
                    yMax = prev;
                if (prev < yMin)
                    yMin = prev;
            }
            signals[i] = new Signal(points, colors[i % 10]);
        }
        let yRange = new Span(-yMin, yMin);
        return [signals, xRange, yRange];
    }
}