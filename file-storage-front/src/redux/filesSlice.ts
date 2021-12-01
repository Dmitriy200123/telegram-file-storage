import {Category, Chat, Sender, TypeFile} from "../models/File";
import {createSlice, PayloadAction} from "@reduxjs/toolkit";
import {fetchChats, fetchFiles, fetchFilters} from "./mainThunks";
import {fetchDownloadLink, fetchFile, fetchRemoveFile} from "./fileThunks";


const initialState = {
    chats: null as null | Array<Chat>,
    senders: null as null | Array<Sender>,
    loading: false,
    error: null as string | null,
    files: [
        {
            fileName: "Файл",
            fileType: Category.документы,
            chat: {
                "id": "c1734b7c-4acf-11ec-81d3-0242ac130003",
                "name": "фуллы",
                "imageId": "d33acc68-4acf-11ec-81d3-0242ac130003"
            },
            fileId: "айди3",
            uploadDate: "12.10.2020",
            downloadLink: "asdasdasd",
            sender: {
                "id": "d33ad0b4-4acf-11ec-81d3-0242ac130003",
                "telegramUserName": "asdasd",
                "fullName": "Кабанщие"
            }
        },
        {
            fileName: "Файл2",
            fileType: Category.картинки,
            chat: {
                "id": "c1734b7c-4acf-11ec-81d3-0242ac130007",
                "name": "фуллы2",
                "imageId": "d33acc68-4acf-11ec-81d3-0242ac130003"
            },
            fileId: "айди2",
            uploadDate: "13.10.2020",
            downloadLink: "asdasdasd",
            sender: {
                "id": "d33ad0b4-4acf-11ec-81d3-0242ac130004",
                "telegramUserName": "asdasd",
                "fullName": "1"
            }
        },
        {
            fileName: "Файл3",
            fileType: Category.аудио,
            chat: {
                "id": "c1734b7c-4acf-11ec-81d3-0242ac130009",
                "name": "фуллы3",
                "imageId": "d33acc68-4acf-11ec-81d3-0242ac130003"
            },
            fileId: "айди1",
            uploadDate: "14.10.2020",
            downloadLink: "asdasdasd",
            sender: {
                "id": "d33ad0b4-4acf-11ec-81d3-0242ac130005",
                "telegramUserName": "asdasd",
                "fullName": "2"
            }
        },
    ] as Array<TypeFile>,
    openFile: null as null | TypeFile | undefined,
    modalConfirm: {
        isOpen: false,
        id: null as null | string,
    },
}

export const filesSlice = createSlice({
    name: "files",
    initialState,
    reducers: {
        clearError(state) {
            state.error = null;
        },
        closeModal(state) {
            state.modalConfirm.isOpen = false;
        },
        openModalConfirm(state, payload:PayloadAction<{ id:string }>) {
            state.modalConfirm.isOpen = true;
            state.modalConfirm.id = payload.payload.id;
        },
        setOpenFile(state, payload:PayloadAction<TypeFile>) {
            state.modalConfirm.isOpen = true;
            state.openFile = payload.payload;
        },
        setOpenFileById(state, payload:PayloadAction<string>) {
            state.modalConfirm.isOpen = true;
            state.openFile = state.files.find((e) => e.fileId === payload.payload);
        },
    },
    extraReducers: {
        [fetchChats.fulfilled.type]: (state, action: PayloadAction<Array<Chat>>) => {
            state.chats = action.payload;
            state.loading =false;
        },
        [fetchChats.pending.type]: (state, action: PayloadAction) => {
            state.loading = true;
        },
        [fetchChats.rejected.type]: (state, action: PayloadAction<string>) => {
            state.error = action.payload
        },

        [fetchFilters.fulfilled.type]: (state, action: PayloadAction<{ chats:Array<Chat>, senders: Array<Sender> }>) => {
            state.loading = false;
            state.chats = action.payload.chats;
            state.senders = action.payload.senders;
        },
        [fetchFilters.pending.type]: (state, action: PayloadAction) => {
            state.loading = true;
        },
        [fetchFilters.rejected.type]: (state, action: PayloadAction<string>) => {
            state.error = action.payload
        },

        [fetchFiles.fulfilled.type]: (state, action: PayloadAction<Array<TypeFile>>) => {
            state.loading = false;
            state.files = action.payload;
        },
        [fetchFiles.pending.type]: (state, action: PayloadAction) => {
            state.loading = true;
        },
        [fetchFiles.rejected.type]: (state, action: PayloadAction<string>) => {
            state.error = action.payload
        },

        [fetchRemoveFile.fulfilled.type]: (state, action: PayloadAction<string>) => {
            state.loading = false;
            state.files = state.files.filter(e => e.fileId !== action.payload);
            state.modalConfirm.isOpen = false;
        },
        [fetchRemoveFile.pending.type]: (state, action: PayloadAction) => {
            state.loading = true;
        },
        [fetchRemoveFile.rejected.type]: (state, action: PayloadAction<string>) => {
            state.modalConfirm.isOpen = false;
            state.error = action.payload
        },

        [fetchFile.fulfilled.type]: (state, action: PayloadAction<TypeFile>) => {
            state.loading = false;
            state.openFile = action.payload;
        },
        [fetchFile.pending.type]: (state, action: PayloadAction) => {
            state.loading = true;
        },
        [fetchFile.rejected.type]: (state, action: PayloadAction<string>) => {
            state.error = action.payload
        },

        [fetchDownloadLink.fulfilled.type]: (state, action: PayloadAction<TypeFile>) => {
            state.loading = false;
        },
        [fetchDownloadLink.pending.type]: (state, action: PayloadAction) => {
            state.loading = true;
        },
        [fetchDownloadLink.rejected.type]: (state, action: PayloadAction<string>) => {
            state.error = action.payload
        },
    }
});


export default filesSlice.reducer;