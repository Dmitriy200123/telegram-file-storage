import React, {memo, ReactComponentElement} from 'react';
import "./Modal.scss"
import {ModalContent} from "../../../models/File";
import {ModalConfirm} from "./ModalConfirm";
import {ModalEdit} from "./ModalEdit";

const Modal:React.FC = memo(({children}) => {
    return (
        <div className={"modal"}>
            <div className={"modal__content"}>
                {children}
            </div>
        </div>
    )
})

export default Modal;


export function hocModal <T extends object>(WrappedComponent: React.FC<T>) {
    return (props: T) => {
        return <div className={"modal"}>
            <div className={"modal__content"}>
                <WrappedComponent {...props}/>
            </div>
        </div>
    }
}


export const modalContents = {
    [ModalContent.Remove]: ModalConfirm,
    [ModalContent.Edit]:ModalEdit,
}