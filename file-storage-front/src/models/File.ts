export type Category = "images" | "video" | "links" | "documents";
export type FormType<TValue> = { label: string, value: TValue };

export type TypeFile = {
    fileId: string,
    fileName: string,
    fileType: Category,
    senderId: string,
    uploadDate:string,
    chatId:string,
    downloadLink?: string
}

export type Chat = {
    id: string,
    name: string,
    imageId: string,
}