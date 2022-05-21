import React from 'react';
import "./Modal.scss"
import {ModalContent} from "../../../models/File";
import {ModalConfirm} from "./ModalConfirm";
import {ModalEdit} from "./ModalEdit";
import {ModalAddClass} from "./ModalAddClass/ModalAddClass";


export const modalContents = {
    [ModalContent.Remove]: ModalConfirm,
    [ModalContent.Edit]: ModalEdit,
    [ModalContent.AddClass]: ModalAddClass
}
