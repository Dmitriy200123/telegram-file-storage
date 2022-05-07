import {createSlice, PayloadAction} from "@reduxjs/toolkit";
import {MessageType, MessageTypeEnum, Rights, TokensType} from "../models/File";
import {updateAuthToken} from "./api/api";
import {fetchRightsCurrentUser, fetchUserCurrent} from "./thunks/rightsThunks";


const initialState = {
    isAuth: false,
    name: "Полина Антоновна",
    hasTelegram: false,
    avatar: null as string | null,
    loading: false,
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
        addMessage(state, payload: PayloadAction<{ type: MessageTypeEnum, value: string }>) {
            state.messages = [...state.messages, payload.payload];
        },
        setLoading(state, payload: PayloadAction<boolean>) {
            state.loading = payload.payload
        },
        setUserCurrent(state, action: PayloadAction<{ name: string, hasTelegram: boolean, avatar: string | null }>) {
            state.hasTelegram = action.payload.hasTelegram;
        },
        setRightsCurrentUser(state, action: PayloadAction<Rights[]>) {
            state.rights = action.payload;
        },

        setIsAuth(state, payload: PayloadAction<boolean>) {
            state.isAuth = payload.payload;
        },
        setTokensToLocalStorage(state, action: PayloadAction<TokensType>) {
            localStorage.setItem("jwtToken", action.payload.jwtToken);
            localStorage.setItem("refreshToken", action.payload.refreshToken);
            updateAuthToken();
        },
        setAuthGitlab(state, action: PayloadAction<TokensType>) {
            localStorage.setItem("jwtToken", action.payload.jwtToken);
            localStorage.setItem("refreshToken", action.payload.refreshToken);
            updateAuthToken();
        },

        setLogoutTelegram(state, payload: PayloadAction<boolean>) {
            state.hasTelegram = payload.payload;
        },
        setLogout() {
            localStorage.removeItem(`oidc.user:${process.env.REACT_APP_HOST}:${process.env.REACT_APP_ID}`);
            sessionStorage.removeItem(`oidc.user:${process.env.REACT_APP_HOST}:${process.env.REACT_APP_ID}`);
            localStorage.removeItem("jwtToken");
            localStorage.removeItem("refreshToken");
        }
    },
    extraReducers: {
        [fetchUserCurrent.fulfilled.type]: (state, action: PayloadAction<{ name: string, hasTelegram: boolean, avatar: string | null }>) => {
            state.name = action.payload.name;
            state.hasTelegram = action.payload.hasTelegram;
            state.avatar = action.payload.avatar;
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
    }
});


export default profileSlice.reducer;
