import React from 'react';
import "./FilesMain.scss"
import {Route, Switch} from "react-router-dom";
import {useAppSelector} from "../../utils/hooks/reduxHooks";
import FilesMain from "./FilesMain";
import {modalContents} from "../utils/Modal/Modal";
import {OpenedFileContainer} from "./File/OpenFileContainer";

const FilesMainContainer = () => {
    const modalConfirm = useAppSelector((state) => state.filesReducer.modalConfirm);
    const {isOpen, id, content, callbackAccept} = modalConfirm;
    const ModalComponent = modalContents[content || 0];
    return (<>
            <Switch>
                <Route exact path={"/files"} component={FilesMain}/>
                <Route path={"/file/:id"} component={OpenedFileContainer}/>
            </Switch>
            {isOpen && id && <ModalComponent id={id} callbackAccept={callbackAccept}/>}
        </>
    );
};


export default FilesMainContainer;
