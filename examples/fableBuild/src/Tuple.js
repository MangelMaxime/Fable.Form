
export function mapFirst(f, x, y) {
    return [f(x), y];
}

export function mapSecond(f, x, y) {
    return [x, f(y)];
}

