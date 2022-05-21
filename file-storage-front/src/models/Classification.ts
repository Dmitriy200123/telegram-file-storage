export type ClassificationType = {
    "id": string,
    "name": string,
    "classificationWords"?: {
        "id": string,
        "value": string,
        "classificationId": string
    }[] | null
}
