import {createSlice, PayloadAction} from "@reduxjs/toolkit";
import {MessageType, MessageTypeEnum} from "../models/File";
import {fetchChats, fetchFiles, fetchFilters} from "./mainThunks";
import {fetchDownloadLink, fetchEditFileName, fetchFile, fetchRemoveFile} from "./fileThunks";
import {fetchIsAuth} from "./profileThunks";


const initialState = {
    isAuth: false,
    name: "Берсерк Справедливович",
    login: "",
    email: "",
    role: "",
    chats: "",
    messages: [] as Array<MessageType>,
}

export const profileSlice = createSlice({
    name: "profile",
    initialState,
    reducers: {
        clearMessage(state, payload: PayloadAction<Number>) {
            state.messages = state.messages.filter((e, i) => i !== payload.payload);
        },

    },
    extraReducers: {
        [fetchIsAuth.fulfilled.type]: (state) => {
            state.isAuth = true;
        },
        [fetchIsAuth.pending.type]: (state) => {
        },
        [fetchIsAuth.rejected.type]: (state) => {
            state.isAuth = false;
        },



        [fetchRemoveFile.fulfilled.type]: (state) => {
            state.messages = [...state.messages, {type: MessageTypeEnum.Message, value: "Успешно удален файл"}];
        },
        [fetchEditFileName.fulfilled.type]: (state) => {
            state.messages = [...state.messages, {type: MessageTypeEnum.Message, value: "Успешно изменено имя файла"}];
        },
        [fetchEditFileName.fulfilled.type]: (state) => {
            state.messages = [...state.messages, {type: MessageTypeEnum.Message, value: "Успешно изменено имя файла"}];
        },



        [fetchChats.rejected.type]: (state, action: PayloadAction<string>) => {
            state.messages = [...state.messages, {type: MessageTypeEnum.Error, value: action.payload}];
        },
        [fetchFilters.rejected.type]: (state, action: PayloadAction<string>) => {
            state.messages = [...state.messages, {type: MessageTypeEnum.Error, value: action.payload}];
        },
        [fetchFiles.rejected.type]: (state, action: PayloadAction<string>) => {
            state.messages = [...state.messages, {type: MessageTypeEnum.Error, value: action.payload}];
        },
        [fetchRemoveFile.rejected.type]: (state, action: PayloadAction<string>) => {
            state.messages = [...state.messages, {type: MessageTypeEnum.Error, value: action.payload}];
        },
        [fetchEditFileName.rejected.type]: (state, action: PayloadAction<string>) => {
            state.messages = [...state.messages, {type: MessageTypeEnum.Error, value: action.payload}];
        },
        [fetchFile.rejected.type]: (state, action: PayloadAction<string>) => {
            state.messages = [...state.messages, {type: MessageTypeEnum.Error, value: action.payload}];
        },
        [fetchDownloadLink.rejected.type]: (state, action: PayloadAction<string>) => {
            state.messages = [...state.messages, {type: MessageTypeEnum.Error, value: action.payload}];
        },
    }
});


export default profileSlice.reducer;