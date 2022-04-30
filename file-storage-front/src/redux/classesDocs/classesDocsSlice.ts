import {createSlice, PayloadAction} from "@reduxjs/toolkit";
import {ClassificationType} from "../../models/Classification";

export type ClassesModalType = "create" | "edit" | "remove";

type stateType = {
    modal: {
        isOpen: boolean,
        type: null | ClassesModalType,
        args?: any
    },
    count: number,
    classifications: ClassificationType[] | null,
    fetchClassifications: boolean,
}

const initialState: stateType = {
    modal: {
        isOpen: false,
        type: null,
    },
    count: 0,
    classifications: null,
    fetchClassifications: false
}

export const classesDocsSlice = createSlice({
    name: "classesDocs",
    initialState,
    reducers: {
        openModal(state, payload: PayloadAction<{ type: ClassesModalType, args?: any }>) {
            state.modal = {
                isOpen: true,
                type: payload.payload.type,
                args: payload.payload.args
            }
        },
        setIsFetchClassifications(state, payload: PayloadAction<boolean>) {
            state.fetchClassifications = payload.payload;
        },
        setCount(state, payload: PayloadAction<number>) {
            state.count = payload.payload;
        },
        setClassifications(state, payload: PayloadAction<ClassificationType[] | null>) {
            state.classifications = payload.payload;
        },
        setClassification(state, payload: PayloadAction<ClassificationType>) {
            state.classifications = state.classifications
                ? [...state.classifications, payload.payload]
                : [payload.payload];
        },
        renameClassification(state, payload: PayloadAction<{ id: string, name: string }>) {
            if (!state.classifications)
                return;
            state.classifications = state.classifications.map(classification => {
                if (classification.id === payload.payload.id)
                    return {...classification, name: payload.payload.name};
                return classification;
            });
        },
        deleteClassification(state, payload: PayloadAction<{ id: string }>) {
            if (!state.classifications)
                return;
            state.classifications = state.classifications.filter(classification => {
                return classification.id !== payload.payload.id;
            });
        },
        addClassificationTag(state, payload: PayloadAction<{ classId: string, tagId: string, value: string }>) {
            if (!state.classifications)
                return;
            state.classifications = state.classifications.map(classification => {
                if (classification.id === payload.payload.classId) {
                    const newWord = {
                        id: payload.payload.tagId,
                        value: payload.payload.value,
                        classificationId: payload.payload.classId
                    };

                    const classificationWords = classification.classificationWords ?
                        [...classification.classificationWords, newWord] : [newWord]
                    return {...classification, classificationWords: classificationWords || null};

                }

                return classification;
            });
        },
        removeClassificationTag(state, payload: PayloadAction<{ classId: string, tagId: string }>) {
            if (!state.classifications)
                return;
            state.classifications = state.classifications.map(classification => {
                if (classification.id === payload.payload.classId) {
                    const classificationWords = classification?.classificationWords?.filter(word => {
                        return word.id !== payload.payload.tagId;
                    })
                    return {...classification, classificationWords: classificationWords || null};
                }

                return classification;
            });
        },
        closeModal(state) {
            state.modal = {isOpen: false, type: null, args: null};
        },
    },
    extraReducers: {}
});


export default classesDocsSlice.reducer;
