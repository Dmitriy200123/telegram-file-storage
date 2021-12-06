import React from 'react';
import "./Modal.scss"
import {ModalContent} from "../../../models/File";
import {ModalConfirm} from "./ModalConfirm";
import {ModalEdit} from "./ModalEdit";

const Modal:React.FC = ({children}) => {
    return (
        <div className={"modal"}>
            <div className={"modal__content"}>
                {children}
            </div>
        </div>
    )
}

export default Modal;


export const modalContents = {
    [ModalContent.Remove]: ModalConfirm,
    [ModalContent.Edit]:ModalEdit,
}