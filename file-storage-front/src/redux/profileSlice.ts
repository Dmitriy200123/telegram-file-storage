import {createSlice, PayloadAction} from "@reduxjs/toolkit";
import {MessageType, MessageTypeEnum, Rights, TokensType} from "../models/File";
import {fetchChats, fetchFiles, fetchFilters} from "./thunks/mainThunks";
import {
    fetchDownloadLink,
    fetchEditFileName,
    fetchFile,
    fetchRemoveFile,
    postCustomFile,
    postFile
} from "./thunks/fileThunks";
import {fetchAuthGitlab, fetchIsAuth, fetchLogout} from "./thunks/profileThunks";
import {updateAuthToken} from "./api/api";
import {fetchRightsCurrentUser, fetchUserCurrent} from "./thunks/rightsThunks";


const initialState = {
    isAuth: false,
    name: "Берсерк Справедливович",
    hasTelegram: false,
    avatar: null as string | null,
    loading: true,
    messages: [] as Array<MessageType>,
    rights: [] as null | Rights[],
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
        [fetchUserCurrent.fulfilled.type]: (state, action: PayloadAction<{ name: string , hasTelegram: boolean, avatar: string | null}>) => {
            state.name = action.payload.name;
            state.hasTelegram = action.payload.hasTelegram;
            state.avatar =  action.payload.avatar;
        },
        [fetchUserCurrent.pending.type]: (state) => {
            state.loading = true;
        },
        [fetchUserCurrent.rejected.type]: (state) => {
            state.loading = false;
        },


        [fetchRightsCurrentUser.fulfilled.type]: (state, action: PayloadAction<Rights[]>) => {
            state.rights = action.payload;
        },
        [fetchRightsCurrentUser.pending.type]: (state) => {
            state.loading = true;
        },
        [fetchRightsCurrentUser.rejected.type]: (state) => {
            state.loading = false;
        },


        [fetchIsAuth.fulfilled.type]: (state, action: PayloadAction<TokensType>) => {
            state.loading = false;
            state.isAuth = true;
            localStorage.setItem("jwtToken", action.payload.jwtToken);
            localStorage.setItem("refreshToken", action.payload.refreshToken);
            updateAuthToken();
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
            updateAuthToken();
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

        [postFile.fulfilled.type]: (state, _) => {
            state.messages = [...state.messages, {type: MessageTypeEnum.Message, value: "Успешно загружен"}];
            state.loading = false;
        },
        [postFile.pending.type]: (state, _) => {
            state.loading = true;
        },
        [postFile.rejected.type]: (state, action: PayloadAction<string>) => {
            state.loading = false;
            state.messages = [...state.messages, {type: MessageTypeEnum.Error, value: action.payload}];
        },

        [postCustomFile.fulfilled.type]: (state, _) => {
            state.messages = [...state.messages, {type: MessageTypeEnum.Message, value: "Успешно загружен"}];
            state.loading = false;
        },
        [postCustomFile.pending.type]: (state, _) => {
            state.loading = true;
        },
        [postCustomFile.rejected.type]: (state, action: PayloadAction<string>) => {
            state.loading = false;
            state.messages = [...state.messages, {type: MessageTypeEnum.Error, value: action.payload}];
        },
    }
});


export default profileSlice.reducer;