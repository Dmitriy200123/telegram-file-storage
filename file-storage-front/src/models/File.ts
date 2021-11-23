export enum Category {
    "documents",
    "video",
    "links",
    "images",
}
export type FormType<TValue> = { label: string, value: TValue };

export type TypeFile = {
    fileId: string,
    fileName: string,
    fileType: Category,
    sender: {
        "id": string,
        "telegramUserName": string,
        "fullName": string
    },
    uploadDate:string,
    chat:{
        "id": string,
        "name": string,
        "imageId"?:string
    },
    downloadLink?: string
}

export type Chat = {
    id: string,
    name: string,
    imageId: string,
}

export type Sender = {
    id: string,
    fullName: string,
    telegramUserName: string,
}