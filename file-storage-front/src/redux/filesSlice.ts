import {Category, File} from "../models/File";
import {createSlice, PayloadAction} from "@reduxjs/toolkit";
import {fetchFiles} from "./ActionsCreators";


const initialState = {
    files: [
        {
            fileName: "Файл",
            fileType: "video",
            chatId: 1,
            fileId: 2,
            uploadDate: "12.10.2020",
            downloadLink: "asdasdasd",
            senderId: 3
        },
        {
            fileName: "Файл2",
            fileType: "images",
            chatId: 2,
            fileId: 3,
            uploadDate: "13.10.2020",
            downloadLink: "asdasdasd",
            senderId: 4
        },
        {
            fileName: "Файл3",
            fileType: "links",
            chatId: 5,
            fileId: 5,
            uploadDate: "14.10.2020",
            downloadLink: "asdasdasd",
            senderId: 5
        },
    ] as Array<File>,
    form: {
        fileName: null as Array<string> | null | undefined,
        date: null as string | null | undefined,
        categories: null as Array<Category> | null | undefined,
        senders: null as Array<number> | null | undefined,
        chats: null as Array<number> | null | undefined,
    }
}

// export type InitialStateType = typeof initialState;
// type ActionsTypes = InferActionsTypes<typeof actions>;

export const filesSlice = createSlice({
    name: "files",
    initialState,
    reducers: {
        changeFilterFileName(state, action: PayloadAction<Array<string> | null | undefined>) {
            state.form.fileName = action.payload;
        },
        changeFilterDate(state, action: PayloadAction<string | null | undefined>) {
            state.form.date = action.payload;
        },
        changeFilterCategories(state, action: PayloadAction<Array<Category> | null | undefined>) {
            state.form.categories = action.payload;
        },
        changeFilterChats(state, action: PayloadAction<Array<number> | null | undefined>) {
            state.form.chats = action.payload;
        },
        changeFilterSenders(state, action: PayloadAction<Array<number> | null | undefined>) {
            state.form.senders = action.payload;
        },
        changeFilters(state, action: PayloadAction<{
            fileName: Array<string> | null | undefined,
            date: string | null | undefined,
            categories: Array<Category> | null | undefined,
            senders: Array<number> | null | undefined,
            chats:Array<number> | null | undefined,
        }>) {
            state.form = action.payload;
        },
    },
    extraReducers: {
        [fetchFiles.fulfilled.type]:(state, action:PayloadAction) => {

        }
    }
});


export default filesSlice.reducer;