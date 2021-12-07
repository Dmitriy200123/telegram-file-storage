export function separateStringExtension(line:string) {
    const index = line.lastIndexOf(".");
    if (index >= 0)
        return [line.slice(0, index), line.slice(index)];
    return null;
}