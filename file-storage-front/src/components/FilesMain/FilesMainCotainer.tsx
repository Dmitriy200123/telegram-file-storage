import React, {useEffect} from 'react';
import "./FilesMain.scss"
import PaginatorNeNorm from '../utils/Paginator/PaginatorNeNorm';
import FragmentFile from "./FragmentFile";
import {useHistory} from "react-router-dom";
import {useAppDispatch, useAppSelector} from "../../utils/hooks/reduxHooks";
import {SubmitHandler, useForm} from "react-hook-form";
import {fetchFiles, fetchFilters} from "../../redux/thunks/mainThunks";
import {AddToUrlQueryParams, GetQueryParamsFromUrl} from "../../utils/functions";
import {Filters} from "./Filters";
import FilesMain from "./FilesMain";
import {modalContents} from "../utils/Modal/Modal";

const FilesMainContainer = () => {
    const modalConfirm = useAppSelector((state) => state.filesReducer.modalConfirm);
    const {isOpen, id, content, callbackAccept} = modalConfirm;
    const ModalComponent = modalContents[content || 0];
    return (<>
            <FilesMain/>
            {isOpen && id && <ModalComponent id={id} callbackAccept={callbackAccept}/>}
        </>
    );
};


export default FilesMainContainer;
