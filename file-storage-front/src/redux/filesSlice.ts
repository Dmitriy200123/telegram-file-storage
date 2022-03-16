import {Chat, ExpandingObject, ModalContent, Sender, TypeFile, TypePaginator} from "../models/File";
import {createSlice, PayloadAction} from "@reduxjs/toolkit";
import {fetchDownloadLink, fetchEditFileName, fetchFile, fetchFileText, fetchRemoveFile} from "./thunks/fileThunks";

const initialState = {
    chats: null as null | Array<Chat>,
    senders: [{id: "123", fullName: "имя", telegramUserName: "телега"}, {
        id: "124",
        fullName: "имя2",
        telegramUserName: "телега"
    }, {id: "125", fullName: "имя3", telegramUserName: "телега"}] as null | Array<Sender>,
    filesNames: null as string[] | null,
    loading: false,
    files: [] as Array<TypeFile>,
    openFile: null as null | (TypeFile & { message?: string }) | undefined,
    modalConfirm: {
        isOpen: false,
        id: null as null | string,
        content: null as null | ModalContent,
        callbackAccept: null as null | AnyFuncType | undefined,
    },
    paginator: {
        count: 1,
        filesInPage: 10,
        currentPage: 1
    } as TypePaginator,
    filesCount: 0,
    filesTypes: {} as ExpandingObject<string>,
};

export const filesSlice = createSlice({
    name: "files",
    initialState,
    reducers: {
        closeModal(state) {
            state.modalConfirm.isOpen = false;
            state.modalConfirm.id = null
            state.modalConfirm.callbackAccept = null;
        },
        openModal(state, payload: PayloadAction<{ id: string, content: ModalContent, callbackAccept?: AnyFuncType }>) {
            state.modalConfirm.isOpen = true;
            state.modalConfirm.id = payload.payload.id;
            state.modalConfirm.content = payload.payload.content;
            state.modalConfirm.callbackAccept = payload.payload.callbackAccept;
        },
        setOpenFile(state, payload: PayloadAction<TypeFile>) {
            state.openFile = payload.payload;
        },
        setOpenFileById(state, payload: PayloadAction<string>) {
            state.modalConfirm.isOpen = true;
            state.openFile = state.files.find((e) => e.fileId === payload.payload);
        },
        changePaginatorPage(state, action: PayloadAction<number>) {
            state.paginator.currentPage = action.payload;
        },
        setLoading(state, action: PayloadAction<boolean>) {
            state.loading = action.payload;
        },

        setFilesTypes(state, action: PayloadAction<Array<{ id: string, name: string }>>) {
            const types: ExpandingObject<string> = {};
            action.payload.forEach(({id, name}) => {
                types[id] = name;
            });

            state.filesTypes = types;
        },

        setFilters(state, action: PayloadAction<{ chats: Array<Chat>, senders: Array<Sender>, countFiles: string | number, filesNames: string[] | null }>) {
            state.chats = action.payload.chats;
            state.senders = action.payload.senders;
            const pagesCount = Math.ceil((+action.payload.countFiles / state.paginator.filesInPage));
            state.filesCount = +action.payload.countFiles;
            state.paginator.count = isNaN(pagesCount) ? 1 : pagesCount;
            state.filesNames = action.payload.filesNames;
        },
        setFiles(state, action: PayloadAction<{ files: Array<TypeFile>, filesCount: string | number }>) {
            state.loading = false;
            state.files = action.payload.files;
            const pagesCount = Math.ceil((+action.payload.filesCount / state.paginator.filesInPage));
            state.filesCount = +action.payload.filesCount;
            state.paginator.count = isNaN(pagesCount) ? 1 : pagesCount;
        },


    },
    extraReducers: {
        [fetchRemoveFile.fulfilled.type]: (state, action: PayloadAction<string>) => {
            state.loading = false;
            state.files = state.files.filter(e => e.fileId !== action.payload);
            state.filesCount--;
            state.paginator.count = Math.ceil((state.filesCount / state.paginator.filesInPage));
            if (state.paginator.currentPage > 0 && state.paginator.currentPage > state.paginator.count)
                state.paginator.currentPage--;
            state.modalConfirm.isOpen = false;
        },

        [fetchRemoveFile.pending.type]: (state) => {
            state.loading = true;
        },

        [fetchRemoveFile.rejected.type]: (state) => {
            state.loading = false;
            state.modalConfirm.isOpen = false;
            state.modalConfirm.id = null
        },

        [fetchEditFileName.fulfilled.type]: (state, action: PayloadAction<{ id: string, fileName: string }>) => {
            state.loading = false;
            state.files = state.files.map(e => e.fileId === action.payload.id ? {
                ...e,
                fileName: action.payload.fileName
            } : e);
            if (state.openFile && state.openFile.fileId === action.payload.id)
                state.openFile.fileName = action.payload.fileName;
            state.modalConfirm.isOpen = false;
        },
        [fetchEditFileName.pending.type]: (state, action: PayloadAction) => {
            state.loading = true;
        },
        [fetchEditFileName.rejected.type]: (state, action: PayloadAction<string>) => {
            state.loading = false;
            state.modalConfirm.isOpen = false;
            state.modalConfirm.id = null
        },

        [fetchFile.fulfilled.type]: (state, action: PayloadAction<TypeFile>) => {
            state.loading = false;
            state.openFile = action.payload;
            if (state.files.length === 0)
                state.files = [action.payload];
        },
        [fetchFile.pending.type]: (state, action: PayloadAction) => {
            state.loading = true;
        },
        [fetchFile.rejected.type]: (state, action: PayloadAction) => {
            state.loading = false;
        },

        [fetchFileText.fulfilled.type]: (state, action: PayloadAction<string>) => {
            state.loading = false;
            if (state.openFile)
                state.openFile.message = action.payload;
        },
        [fetchFileText.pending.type]: (state, action: PayloadAction) => {
            state.loading = true;
        },
        [fetchFileText.rejected.type]: (state, action: PayloadAction) => {
            state.loading = false;
        },


        [fetchDownloadLink.fulfilled.type]: (state, action: PayloadAction<TypeFile>) => {
            state.loading = false;
        },
        [fetchDownloadLink.pending.type]: (state, action: PayloadAction) => {
            state.loading = true;
        },
        [fetchDownloadLink.rejected.type]: (state, action: PayloadAction) => {
            state.loading = false;
        },

        //endregion
    }
});


type AnyFuncType = (...args: any) => void;

export const filesSliceActions = filesSlice.actions;
export default filesSlice.reducer;