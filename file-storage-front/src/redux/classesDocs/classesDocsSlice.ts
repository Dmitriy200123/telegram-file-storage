import {createSlice, PayloadAction} from "@reduxjs/toolkit";

export type ClassesModalType = "create" | "edit" | "remove";

type stateType = {
    modal: {
        isOpen: boolean,
        type: null | ClassesModalType,
        args?: any
    }
}

const initialState: stateType = {
    modal: {
        isOpen: false,
        type: null,
    }
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

        closeModal(state) {
            state.modal = {isOpen: false, type: null, args: null};
        },
    },
    extraReducers: {}
});


export default classesDocsSlice.reducer;
