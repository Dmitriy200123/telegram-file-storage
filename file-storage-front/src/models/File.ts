export type Category = "images" | "video" | "links" | "documents";
export type FormType<TValue> = { label: string, value: TValue };

export type File = {
    fileId: number,
    fileName: string,
    fileType: Category,
    senderId: number,
    uploadDate:string,
    chatId:number,
    downloadLink: string
}

export type Chat = {
    id: string,
    name: string,
    imageId: string,
}