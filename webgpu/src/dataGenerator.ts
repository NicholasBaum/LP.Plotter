import { getTestRun } from "./csvParser";
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
        signals[0] = new Signal(points, colors[0], 1, yRange);
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

    // adds random thickness and yViewRanges
    static createMultiLineData2(seriesCount = 500, sampleCount = 500): [signals: Signal[], xRange: Span, yRange: Span] {
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
            signals[i] = new Signal(points, colors[i % 10], i % 3 + 1, new Span(-1, 1));
        }
        for (var i = 0; i < seriesCount; i++) {
            let f = i % 3 + 1;
            let ySpan = new Span(-yMin, yMin);
            ySpan.zoomRelative(f, 0.5);
            signals[i].yViewRange = ySpan;
        };
        return [signals, xRange, new Span(-1, 1)];
    }

    static createChannelData(index: number): [signals: Signal[], xRange: Span, yRange: Span] {
        return this.createRunData(index, index);
    }

    static createRunData(indexMin: number, indexMax: number): [signals: Signal[], xRange: Span, yRange: Span] {
        const data = getTestRun();
        const xRange = new Span(data[0].samples[0].x, data[0].samples[data[0].samples.length - 1].x);
        let signals: Signal[] = [];
        for (let i = indexMin; i <= Math.min(data.length, indexMax); i++) {
            const samples = data[i].samples;
            var min = samples.reduce((a, b) => a.y <= b.y ? a : b).y;
            var max = samples.reduce((a, b) => a.y > b.y ? a : b).y;
            let yRange = new Span(min, max);
            console.log(data[i].name);
            console.log(yRange);
            let s = new Signal(data[i].samples, colors[i % 10], 1, yRange);
            signals.push(s);
        }
        return [signals, xRange, new Span(-1, 1)];
    }
}