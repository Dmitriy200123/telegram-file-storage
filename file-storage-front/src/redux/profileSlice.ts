import {createSlice, PayloadAction} from "@reduxjs/toolkit";
import {MessageType, MessageTypeEnum, TokensType} from "../models/File";
import {fetchChats, fetchFiles, fetchFilters} from "./mainThunks";
import {fetchDownloadLink, fetchEditFileName, fetchFile, fetchRemoveFile} from "./fileThunks";
import {fetchAuthGitlab, fetchIsAuth, fetchLogout} from "./profileThunks";


const initialState = {
    isAuth: false,
    name: "Берсерк Справедливович",
    login: "",
    email: "",
    role: "",
    chats: "",
    loading: true,
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
        [fetchIsAuth.fulfilled.type]: (state, action: PayloadAction<TokensType>) => {
            state.loading = false;
            state.isAuth = true;
            localStorage.setItem("jwtToken", action.payload.jwtToken);
            localStorage.setItem("refreshToken", action.payload.refreshToken);
        },
        [fetchIsAuth.pending.type]: (state) => {
            state.loading = true;
        },
        [fetchIsAuth.rejected.type]: (state) => {
            state.loading = false;
            state.isAuth = false;
        },

        [fetchAuthGitlab.fulfilled.type]: (state, action: PayloadAction<TokensType>) => {
            state.isAuth = true;
            localStorage.setItem("jwtToken", action.payload.jwtToken);
            localStorage.setItem("refreshToken", action.payload.refreshToken);
        },
        [fetchAuthGitlab.pending.type]: (state, action) => {
        },
        [fetchAuthGitlab.rejected.type]: (state, action) => {
            state.isAuth = false;
        },

        [fetchLogout.fulfilled.type]: (state, action: PayloadAction<TokensType>) => {
            state.loading = false;
            state.isAuth = false;
            localStorage.removeItem("oidc.user:https://git.66bit.ru:392b8f8766b8da0f5f64edaa50b89b633d302ab0fd7f94aa482d5510e1a97cda");
            sessionStorage.removeItem("oidc.user:https://git.66bit.ru:392b8f8766b8da0f5f64edaa50b89b633d302ab0fd7f94aa482d5510e1a97cda");
            localStorage.removeItem("jwtToken");
            localStorage.removeItem("refreshToken");
        },
        [fetchLogout.pending.type]: (state, action) => {
            state.loading = true;
        },
        [fetchLogout.rejected.type]: (state, action) => {
            state.loading = false;
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