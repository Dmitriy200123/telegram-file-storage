import {Chat, ExpandingObject, ModalContent, Sender, TypeFile, TypePaginator} from "../models/File";
import {createSlice, PayloadAction} from "@reduxjs/toolkit";


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
            if (state.files.length === 0)
                state.files = [payload.payload];
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
        setClassificationWord(state, action: PayloadAction<{fileId: string, classification: {id: string, name: string}}>){
           state.files = state.files.map( e => {
                if (e.fileId === action.payload.fileId){
                    return {...e, classification: action.payload.classification}
                }
                return e;
            } )
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
            state.files = action.payload.files;
            const pagesCount = Math.ceil((+action.payload.filesCount / state.paginator.filesInPage));
            state.filesCount = +action.payload.filesCount;
            state.paginator.count = isNaN(pagesCount) ? 1 : pagesCount;
        },
        changeFileName(state, action: PayloadAction<{ id: string, fileName: string }>) {
            state.files = state.files.map(e => e.fileId === action.payload.id ? {
                ...e,
                fileName: action.payload.fileName
            } : e);

            if (state.openFile && state.openFile.fileId === action.payload.id)
                state.openFile.fileName = action.payload.fileName;
        },
        removeFile(state, action: PayloadAction<string>) {
            state.files = state.files.filter(e => e.fileId !== action.payload);
            state.filesCount--;
            state.paginator.count = Math.ceil((state.filesCount / state.paginator.filesInPage));
            if (state.paginator.currentPage > 0 && state.paginator.currentPage > state.paginator.count)
                state.paginator.currentPage--;
        },
        setFileUrl(state, action: PayloadAction<{ id: string, url: string }>) {
            if (state.openFile && state.openFile.fileId === action.payload.id) {
                state.openFile.url = action.payload.url;
            }
        }

    },

    extraReducers: {}
})


type AnyFuncType = (...args: any) => void;

export const filesSliceActions = filesSlice.actions;
export default filesSlice.reducer;