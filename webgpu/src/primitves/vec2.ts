export class Vec2 {
    constructor(public x: number, public y: number) { }

    length() {
        return Math.sqrt(this.x * this.x + this.y * this.y);
    }

    mult(scalar: number) {
        return new Vec2(this.x * scalar, this.y * scalar);
    }

    dotmult(otherVec2: Vec2) {
        return new Vec2(this.x * otherVec2.x, this.y * otherVec2.y);
    }

    dotdiv(otherVec2: Vec2) {
        return new Vec2(this.x / otherVec2.x, this.y / otherVec2.y);
    }

    div(scalar: number) {
        return new Vec2(this.x / scalar, this.y / scalar);
    }

    add(otherVec2: Vec2) {
        return new Vec2(this.x + otherVec2.x, this.y + otherVec2.y);
    }

    sub(otherVec2: Vec2) {
        return new Vec2(this.x - otherVec2.x, this.y - otherVec2.y);
    }

    normperp() {
        return new Vec2(-this.y, this.x).div(this.length());
    }

    norm() {
        return this.div(this.length());
    }

    dot(otherVec2: Vec2) {
        return this.x * otherVec2.x + this.y * otherVec2.y;
    }

    equals(other: Vec2): boolean {
        return this.x == other.x && this.y == other.y;
    }
}