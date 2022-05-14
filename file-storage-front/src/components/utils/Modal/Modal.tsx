import React, {memo, MouseEventHandler, useRef} from 'react';
import "./Modal.scss"
import {ModalContent} from "../../../models/File";
import {ModalConfirm} from "../../FilesMain/Modals/ModalConfirm";
import {ModalEdit} from "../../FilesMain/Modals/ModalEdit";
import {ModalAddClass} from "../../FilesMain/Modals/ModalAddClass/ModalAddClass";

type PropsType = {
    onOutsideClick?: () => void
    className?: string
}
const Modal: React.FC<PropsType> = memo(({children, onOutsideClick, className}) => {
    const ref = useRef<HTMLDivElement>(null);
    const onOutClick: MouseEventHandler<HTMLDivElement> = (event) => {
        if (ref.current && !ref.current.contains(event.target as Node)) {
            onOutsideClick?.();
        }
    };

    return (
        <div className={"modal"} onClick={onOutClick}>
            <div className={"modal__content" + (className ? ` ${className}` : "")} ref={ref}>
                {children}
            </div>
        </div>
    )
})

export default Modal;


export function hocModal<T extends object>(WrappedComponent: React.FC<T>) {
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
    [ModalContent.Edit]: ModalEdit,
    [ModalContent.AddClass]: ModalAddClass
}
