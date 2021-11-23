import React, {memo, useState} from 'react';
import "./FilesMain.scss"
import {Category, TypeFile} from "../../models/File";
import {Link} from 'react-router-dom';
import {OutsideAlerter} from "../utils/OutSideAlerter/OutSideAlerter";
import {ReactComponent as Edit} from "./../../assets/edit.svg";
import {ReactComponent as Download} from "./../../assets/download_2.svg";
import {ReactComponent as Delete} from "./../../assets/delete.svg";
import {useDispatch} from "react-redux";
import {filesSlice} from "../../redux/filesSlice";

const FragmentFile: React.FC<PropsType> = ({fileId, fileName, uploadDate, fileType, sender, chat}) => {
    return <React.Fragment key={fileId}>
        <Link className={"files__item files__item_name"} to={"/file"} replace>{fileName}</Link>
        <div className={"files__item"}>{uploadDate}</div>
        <div className={"files__item"}>{Category[fileType]}</div>
        <div className={"files__item"}>{sender.fullName}</div>
        <div className={"files__item files__item_relative"}>{chat.name} <Controls id={fileId}/></div>
    </React.Fragment>
};


const {openModalConfirm} = filesSlice.actions
const Controls = memo(({id}:{id: string}) => {
    const [isOpen, changeIsOpen] = useState(false);
    const dispatch = useDispatch();
    return <OutsideAlerter onOutsideClick={() => changeIsOpen(false)}>
        <div className={"file-controls"}>
            <button onClick={(e) => {
                e.preventDefault();
                changeIsOpen(true);
            }} className={"file-controls__btn"}>
                <div className={"file-controls__circle"}/>
            </button>
            {isOpen && <section className={"file-controls__modal"}>
                <div className={"file-controls__modal-item"}><Edit/><span>Переименовать</span></div>
                <div className={"file-controls__modal-item"}><Download/><span>Скачать</span></div>
                <div className={"file-controls__modal-item file-controls__modal-item_delete"}
                     onClick={() => dispatch(openModalConfirm({id}))}>
                    <Delete/><span>Удалить</span></div>
            </section>}
        </div>
    </OutsideAlerter>
});

type PropsType = TypeFile;

export default FragmentFile;


