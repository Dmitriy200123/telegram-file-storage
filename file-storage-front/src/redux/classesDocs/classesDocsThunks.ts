import {fetchConfig, fetchConfigText} from "../api/api";
import {AppDispatch} from "../redux-store";
import {classesDocsSlice} from "./classesDocsSlice";
import {ClassificationType} from "../../models/Classification";

const {setCount, setClassifications, renameClassification, deleteClassification} = classesDocsSlice.actions;

export const fetchCountClassifications = (query?: string) => async (dispatch: AppDispatch) => {
    // dispatch(setLoading(true));
    try {
        const count = +await fetchConfigText(`/api/documentClassifications/count`, {method: "GET", params: {query}});
        dispatch(setCount(count));
    } catch (err) {
        // dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось загрузить файл"}))
    } finally {
        // dispatch(setLoading(false));
    }
};

type FetchClassificationsType = {
    skip: number,
    take: number,
    query?: string,
    includeClassificationWords?:boolean
}

export const fetchClassifications = (args: FetchClassificationsType) => async (dispatch: AppDispatch) => {
    // dispatch(setLoading(true));
    if (!args.includeClassificationWords) {
        args.includeClassificationWords = true;
    }

    try {
        const classifications: ClassificationType[] = await fetchConfig(`/api/documentClassifications`, {
            method: "GET",
            params: args
        });
        dispatch(setClassifications(classifications));
    } catch (err) {
        // dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось загрузить файл"}))
    } finally {
        // dispatch(setLoading(false));
    }
};

export const fetchRenameClassification = ({id, name}: {id:string, name: string}) => async (dispatch: AppDispatch) => {
    // dispatch(setLoading(true));
    try {
        await fetchConfigText(`/api/documentClassifications/${id}`, {
            method: "PUT",
            body: name
        });
        dispatch(renameClassification({id, name}));
    } catch (err) {
        // dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось загрузить файл"}))
    } finally {
        // dispatch(setLoading(false));
    }
};

export const fetchDeleteClassification = ({id}: {id:string}) => async (dispatch: AppDispatch) => {
    // dispatch(setLoading(true));
    try {
        await fetchConfigText(`/api/documentClassifications/${id}`, {
            method: "DELETE",
        });
        dispatch(deleteClassification({id}));
    } catch (err) {
        // dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось загрузить файл"}))
    } finally {
        // dispatch(setLoading(false));
    }
};

export const addClassifications = (classification: Omit<ClassificationType, "id">) => async (dispatch: AppDispatch) => {
    // dispatch(setLoading(true));
    try {
        await fetchConfig(`/api/documentClassifications`, {
            method: "POST",
            body: classification
        });
    } catch (err) {
        // dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось загрузить файл"}))
    } finally {
        // dispatch(setLoading(false));
    }
};


export const addToClassificationWord = (args: { id: string, value: string }) => async (dispatch: AppDispatch) => {
    // dispatch(setLoading(true));
    try {
        await fetchConfig(`/api/documentClassifications/${args.id}/words`, {
            method: "POST",
            body: args.value
        });
        // dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось загрузить файл"}))
    } catch (err) {
        // dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось загрузить файл"}))
    } finally {
        // dispatch(setLoading(false));
    }
};
