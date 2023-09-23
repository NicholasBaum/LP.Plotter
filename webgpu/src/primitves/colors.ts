export class Color {

    get byteLength(): number {
        return 16;
    }

    toFloats32(): number[] {
        return [this.r, this.g, this.b, this.a];
    }

    constructor(
        public readonly r: number,
        public readonly g: number,
        public readonly b: number,
        public readonly a: number) { }
}

export const colors = [
    new Color(1.0, 0.0, 0.0, 1.0),   // Red
    new Color(0.0, 1.0, 0.0, 1.0),   // Green
    new Color(0.0, 0.0, 1.0, 1.0),   // Blue
    new Color(1.0, 1.0, 0.0, 1.0),   // Yellow
    new Color(1.0, 0.0, 1.0, 1.0),   // Magenta
    new Color(0.0, 1.0, 1.0, 1.0),   // Cyan
    new Color(0.5, 0.5, 0.5, 1.0),   // Gray
    new Color(1.0, 0.5, 0.0, 1.0),   // Orange
    new Color(0.0, 0.5, 1.0, 1.0),   // Light Blue
    new Color(0.5, 1.0, 0.5, 1.0),   // Light Green
];