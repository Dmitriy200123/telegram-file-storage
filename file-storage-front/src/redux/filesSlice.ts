import {Chat, TypeFile} from "../models/File";
import {createSlice, PayloadAction} from "@reduxjs/toolkit";
import {fetchChats, fetchFilters} from "./ActionsCreators";


const initialState = {
    chats: null as null | Array<Chat>,
    senders: null as null | Array<string>,
    loading: false,
    error: null as string | null,
    files: [
        {
            fileName: "Файл",
            fileType: "video",
            chatId: "Я АЙДИШНИК ТУТУТУТУТУ",
            fileId: "айди3",
            uploadDate: "12.10.2020",
            downloadLink: "asdasdasd",
            senderId: "айдиSender"
        },
        {
            fileName: "Файл2",
            fileType: "images",
            chatId: "айди",
            fileId: "айди2",
            uploadDate: "13.10.2020",
            downloadLink: "asdasdasd",
            senderId: "айдиJOJO"
        },
        {
            fileName: "Файл3",
            fileType: "links",
            chatId: "айди1",
            fileId: "айди1",
            uploadDate: "14.10.2020",
            downloadLink: "asdasdasd",
            senderId: "айдикАБАН"
        },
    ] as Array<TypeFile>,
    modalConfirm: {
        isOpen: false,
        id: null as null | string,
    },
    some: null as any
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
    },
    extraReducers: {
        [fetchChats.fulfilled.type]: (state, action: PayloadAction<Array<Chat>>) => {
            state.chats = action.payload;
        },
        [fetchChats.pending.type]: (state, action: PayloadAction) => {
            state.loading = true;
        },
        [fetchChats.rejected.type]: (state, action: PayloadAction<string>) => {
            state.error = action.payload
        },

        [fetchFilters.fulfilled.type]: (state, action: PayloadAction<{ chats:Array<Chat>, senders: Array<string> }>) => {
            state.loading = true;
            state.chats = action.payload.chats;
            state.senders = action.payload.senders;
        },
        [fetchFilters.pending.type]: (state, action: PayloadAction) => {
            state.loading = false;
        },
        [fetchFilters.rejected.type]: (state, action: PayloadAction<string>) => {
            state.error = action.payload
        },
    }
});


export default filesSlice.reducer;