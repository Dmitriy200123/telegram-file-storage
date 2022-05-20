import {fetchConfig, fetchConfigText} from "../api/apiFiles";
import {AppDispatch} from "../redux-store";
import {filesSliceActions} from "../filesSlice";
import {profileSlice} from "../profileSlice";
import {MessageTypeEnum} from "../../models/File";

const {setClassification, setFilesTypes, setLoading, setFilters, setFiles} = filesSliceActions;
const {addMessage} = profileSlice.actions;


export const fetchFilters = () => async (dispatch: AppDispatch) => {
    try {
        dispatch(setLoading(true));
        const chats = fetchConfig("/api/chats");
        const senders = fetchConfig("/api/senders");
        const filesNames = fetchConfig("/api/files/names");
        const countFiles = fetchConfigText("/api/files/count");
        dispatch(setFilters({
            chats: await chats,
            senders: await senders,
            countFiles: await countFiles,
            filesNames: await filesNames
        }));
        dispatch(setLoading(false));
    } catch (err) {
        dispatch(setLoading(false));
        dispatch(addMessage({type:MessageTypeEnum.Error,value:"Не удалось загрузить фильтры"}));
    }
};

type TypeFilesFetchFilters = {
    skip: number,
    take: number,
    fileName?: string | null,
    categories?: string[] | null | undefined,
    dateTo?: string | null | undefined,
    dateFrom?: string | null | undefined,
    senderIds?: string[] | null | undefined,
    chatIds?: string[] | null | undefined,
}

export const fetchFiles = (args:TypeFilesFetchFilters) => async (dispatch: AppDispatch) => {
    try {
        const params: any = {...args};
        delete params['take'];
        delete params['skip'];
        dispatch(setLoading(true));
        const filesCount = fetchConfigText("/api/files/count", {params: params})
        dispatch(setFiles({files: await fetchConfig(`/api/files`, {params: args}), filesCount: await filesCount}));
        dispatch(setLoading(false));
    } catch (err) {
        dispatch(setLoading(false));
        dispatch(addMessage({type:MessageTypeEnum.Error,value:"Не удалось загрузить файлы"}))
    }
};

type TypeDocumentsFetchFilters = {
    skip: number,
    take: number,
    phrase?: string | null,
    categories?: string[] | null | undefined,
    dateTo?: string | null | undefined,
    dateFrom?: string | null | undefined,
    senderIds?: string[] | null | undefined,
    chatIds?: string[] | null | undefined,
    classificationIds?: string[] | null
}

export const fetchDocuments = (args:TypeDocumentsFetchFilters) => async (dispatch: AppDispatch) => {
    try {
        const params: any = {...args};
        delete params['take'];
        delete params['skip'];
        dispatch(setLoading(true));
        const filesCount = fetchConfigText("/api/files/documents/count", {params: params})
        const documents = (await fetchConfig(`/api/files/documents`, {params: args}))
            .map((e:any) => ({
                fileName: e.documentName,
                sender: e.sender,
                fileId: e.documentId,
                fileType: 6,
                uploadDate: e.uploadDate,
                chat: e.chat,
            }));
        dispatch(setFiles({files: documents, filesCount: await filesCount}));
        dispatch(setLoading(false));
    } catch (err) {
        dispatch(setLoading(false));
        dispatch(addMessage({type:MessageTypeEnum.Error,value:"Не удалось загрузить документы"}))
    }
};

export const fetchFilesTypes = () => async (dispatch: AppDispatch) => {
    try {
        dispatch(setLoading(true));
        dispatch(setFilesTypes(await fetchConfig(`/api/files/types`)));
        dispatch(setLoading(false));
    } catch (err) {
        dispatch(setLoading(false));
        dispatch( addMessage({type:MessageTypeEnum.Error,value:"Не удалось загрузить типы файлов"}))
    }
}

export const fetchClassification = (id: string) => async (dispatch: AppDispatch) => {
    try {
        dispatch(setLoading(true));
        const response = await fetchConfig(`/api/files/documents/${id}/classification`);
        dispatch(setClassification({classification: response, fileId: id}))
    } catch (e) {
        //TODO добавить error или не надо?
    } finally {
        dispatch(setLoading(false));
    }
}

