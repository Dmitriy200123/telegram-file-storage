import React, {FC} from 'react';
import Modal from "../../../utils/Modal/Modal";
import classes from "./DocsClassesModal.module.scss";
import Edit from "../Edit/Edit";
import Create from "../Create/Create";
import Remove from "../Remove/Remove";
import {ClassesModalType} from "../../../../redux/classesDocs/classesDocsSlice";

const modalsContent = {
    "edit": Edit,
    "remove": Remove,
    "create": Create
} as const;

type PropsType = {
    onOutsideClick?: () => void,
    modalType: ClassesModalType,
    args?: any
}

const DocsClassesModal: FC<PropsType> = ({onOutsideClick, modalType, args}) => {
    const Content = modalsContent[modalType]
    return (<Modal onOutsideClick={onOutsideClick} className={classes.content}>
            <Content args={args} onOutsideClick={onOutsideClick} />
        </Modal>
    );
};


export default DocsClassesModal;
