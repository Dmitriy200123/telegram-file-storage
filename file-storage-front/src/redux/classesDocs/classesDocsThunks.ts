import {fetchConfig, fetchConfigText} from "../api/apiDocsClasses";
import {AppDispatch} from "../redux-store";
import {classesDocsSlice} from "./classesDocsSlice";
import {ClassificationType} from "../../models/Classification";

const {setCount, setClassifications, renameClassification, deleteClassification, setClassification, closeModal,
removeClassificationTag, addClassificationTag, } = classesDocsSlice.actions;

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
    includeClassificationWords?: boolean
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

export const fetchClassification = (id:string ) => async (dispatch: AppDispatch) => {
    try {
        const classification: ClassificationType = await fetchConfig(`/api/documentClassifications/${id}`, {
            method: "GET",
            params: {includeClassificationWords: true}
        });

        dispatch(setClassification(classification));
    } catch (err) {
        // dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось загрузить файл"}))
    } finally {
        // dispatch(setLoading(false));
    }
};


export const fetchRenameClassification = ({
                                              id,
                                              name
                                          }: { id: string, name: string }) => async (dispatch: AppDispatch) => {
    // dispatch(setLoading(true));
    try {
        await fetchConfigText(`/api/documentClassifications/${id}`, {
            method: "PUT",
            body: name
        });
        dispatch(renameClassification({id, name}));
        dispatch(closeModal());
    } catch (err) {
        // dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось загрузить файл"}))
    } finally {
        // dispatch(setLoading(false));
    }
};

export const fetchDeleteClassification = ({id}: { id: string }) => async (dispatch: AppDispatch) => {
    // dispatch(setLoading(true));
    try {
        await fetchConfigText(`/api/documentClassifications/${id}`, {
            method: "DELETE",
        });
        dispatch(deleteClassification({id}));
        dispatch(closeModal());
    } catch (err) {
        // dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось загрузить файл"}))
    } finally {
        // dispatch(setLoading(false));
    }
};

type PostClassType = { "name": string, "classificationWords": { "value": string }[] }

export const addClassification = (classification: PostClassType) => async (dispatch: AppDispatch) => {
    // dispatch(setLoading(true));
    try {
        await fetchConfigText(`/api/documentClassifications`, {
            method: "POST",
            body: classification
        });

        // @ts-ignore
        dispatch(setClassifications(classification))
        dispatch(closeModal())
    } catch (err) {
        // dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось загрузить файл"}))
    } finally {
        // dispatch(setLoading(false));
    }
};


export const postAddToClassificationWord = (args: { classId: string, value: string }) => async (dispatch: AppDispatch) => {
    // dispatch(setLoading(true));
    try {
        await fetchConfigText(`/api/documentClassifications/${args.classId}/words`, {
            method: "POST",
            body: {value: args.value}
        });
        dispatch(addClassificationTag({...args, tagId: "1"}))
        // dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось загрузить файл"}))
    } catch (err) {
        // dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось загрузить файл"}))
    } finally {
        // dispatch(setLoading(false));
    }
};

export const fetchDeleteToClassificationWord = (args: { classId: string, tagId: string}) => async (dispatch: AppDispatch) => {
    // dispatch(setLoading(true));
    try {
        await fetchConfigText(`/api/documentClassifications/words/${args.tagId}`, {
            method: "DELETE",
        });
        dispatch(removeClassificationTag(args))
        // dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось загрузить файл"}))
    } catch (err) {
        // dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось загрузить файл"}))
    } finally {
        // dispatch(setLoading(false));
    }
};
