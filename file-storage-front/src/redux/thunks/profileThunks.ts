import {TokensType} from "../../models/File";
import {fetchAuth, fetchLog} from "../api/api-auth";
import {fetchConfigText} from "../api/apiFiles";
import {AppDispatch} from "../redux-store";
import {profileSlice} from "../profileSlice";
import {MessageTypeEnum} from "../../models/File";

const {addMessage, setLoading, setAuthGitlab, setTokensToLocalStorage, setIsAuth} = profileSlice.actions;

export const fetchIsAuth = () => async (dispatch: AppDispatch) => {
    const jwtToken = localStorage.getItem("jwtToken");
    const refreshToken = localStorage.getItem("refreshToken");
    if (jwtToken === null || refreshToken === null) {
        return dispatch(setIsAuth(false));
    }

    try {
        dispatch(setLoading(true));
        const tokens: TokensType = {jwtToken, refreshToken};
        const data: TokensType = await fetchAuth("/auth/gitlab/refresh", {body: tokens, method: "POST"});
        dispatch(setIsAuth(true));
        dispatch(setTokensToLocalStorage(data));
    } catch (e) {
        dispatch(setLoading(false));
        dispatch(setIsAuth(false));
    }
}

export const fetchAuthGitlab = (token: TokensType) => async (dispatch: AppDispatch) => {
    try {
        const data: TokensType = await fetchAuth("/auth/gitlab", {token: token});
        dispatch(setAuthGitlab(data));
        dispatch(setIsAuth(true));
    } catch (e) {
        dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось войти с помощью гитлаба"}));
    }
}

export const fetchLogout = () => async (dispatch: AppDispatch) => {
    try {
        dispatch(setLoading(true));
        dispatch(setIsAuth(false))
        localStorage.setItem("flag", "false");
        sessionStorage.removeItem(`oidc.user:${process.env.REACT_APP_HOST}:${process.env.REACT_APP_ID}`)
        localStorage.removeItem(`oidc.user:${process.env.REACT_APP_HOST}:${process.env.REACT_APP_ID}`)
        await fetchLog("/auth/gitlab/logout");
    } catch (e) {
        dispatch(setLoading(false));
        dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не войти с помощью гитлаба"}));
    } finally {
        dispatch(setLoading(false));
    }
}

export const fetchLogoutTelegram = () => async (dispatch: AppDispatch) => {
    try {
        dispatch(setLoading(true));
        await fetchConfigText("/auth/telegram/logout", {method: "POST"});
    } catch (e) {
        dispatch(setLoading(false));
        dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось выйти"}));

    }
}
