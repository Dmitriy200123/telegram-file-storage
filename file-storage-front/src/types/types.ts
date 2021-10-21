export type Category = "images" | "video" | "links" | "documents";

export type File = {
    fileId: number,
    fileName: string,
    fileType: Category,
    senderId: number,
    uploadDate:string,
    chatId:number,
    downloadLink: string
}