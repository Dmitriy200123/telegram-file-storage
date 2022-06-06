import React, {FC, memo, useEffect, useState} from 'react';
import "./FilesMain.scss"
import {ExpandingObject, ModalContent, Rights, TypeFile} from "../../models/File";
import {Link} from 'react-router-dom';
import {OutsideAlerter} from "../utils/OutSideAlerter/OutSideAlerter";
import {ReactComponent as Edit} from "./../../assets/edit.svg";
import {ReactComponent as Download} from "./../../assets/download_2.svg";
import {ReactComponent as Delete} from "./../../assets/delete.svg";
import {ReactComponent as Reject} from "./../../assets/reject.svg";
import {ReactComponent as Tag} from "./../../assets/tag.svg";
import {useDispatch} from "react-redux";
import {filesSlice} from "../../redux/filesSlice";
import {Dispatch} from "@reduxjs/toolkit";
import {fetchClassification} from "../../redux/thunks/mainThunks";
import {deleteClassificationDocument, fetchDownloadLinkAndDownloadFile} from "../../redux/thunks/fileThunks";
import {ClassificationType} from "../../models/Classification";

const {openModal, setOpenFile} = filesSlice.actions

const FragmentFile: React.FC<PropsType> = ({file, rights, types, fetchFiles}) => {
    const {fileId, fileName, uploadDate, fileType, sender, chat, classification} = file;
    const dispatch = useDispatch();
    useEffect(() => {
        if (!classification && +file.fileType === 6 && rights?.includes(Rights["Поиск классификаций"])) {
            dispatch(fetchClassification(file.fileId))
        }
    }, [classification])
    return <React.Fragment key={fileId}>
        <div className={"files__nameClassItems"}>
            <Link className={"files__item_name"} to={`/file/${fileId}`} replace onClick={() => {
                dispatch(setOpenFile(file));
            }}>{fileName}</Link>

            {classification && <div className={"files__item_classf"}>{classification.name}</div>}
        </div>
        <div className={"files__item"}>{uploadDate}</div>
        <div className={"files__item"}>{types && types[fileType]}</div>
        <div className={"files__item"}>{sender.fullName}</div>
        <div className={"files__item files__item_relative"}>{chat?.name}
            <Controls rights={rights} id={fileId}
                      fileType={fileType}
                      dispatch={dispatch}
                      fetchFiles={fetchFiles}
                      classification={classification}
            />
        </div>
    </React.Fragment>
};


const Controls: FC<ControlsPropsType> = memo(({
                                                  id,
                                                  dispatch,
                                                  rights,
                                                  fileType,
                                                  fetchFiles,
                                                  classification
                                              }) => {
    const [isOpen, changeIsOpen] = useState(false);

    function onRevokeClass() {
        if (!classification)
            return;
        dispatch(deleteClassificationDocument({
            documentId: id,
            classId: classification.id
        }));
    }
    return <OutsideAlerter onOutsideClick={() => changeIsOpen(false)}>
        <div className={"file-controls"}>
            <button onClick={(e) => {
                e.preventDefault();
                changeIsOpen(true);
            }} className={"file-controls__btn"}>
                <div className={"file-controls__circle"}/>
            </button>
            {isOpen && <section className={"file-controls__modal"}>
                {rights?.includes(Rights["Переименовывать файлы"]) &&
                <div className={"file-controls__modal-item"}
                     onClick={() => dispatch(openModal({id, content: ModalContent.Edit}))}>
                    <Edit/><span>Переименовать</span></div>}
                {+fileType !== 4 && +fileType !== 5 &&
                <div className={"file-controls__modal-item"}
                     onClick={() => dispatch(fetchDownloadLinkAndDownloadFile(id))}>
                    <Download/><span>Скачать</span></div>}
                {rights?.includes(Rights["Присвоение классификаций"]) && +fileType === 6 && rights?.includes(Rights["Поиск классификаций"]) && <div className={"file-controls__modal-item file-controls__modal-itemClassSvg"}
                                         onClick={() => dispatch(openModal({id, content: ModalContent.AddClass}))}>
                    <Tag/><span>Присвоить классификацию</span></div>}
                { rights?.includes(Rights["Отзыв классификаций"]) && +fileType === 6 && classification && <div className={"file-controls__modal-item file-controls__modal-itemClassSvg"}
                                         onClick={onRevokeClass}>
                    <Reject/><span>Отозвать классфикацию</span></div>}
                {/*todo classId*/}
                {rights?.includes(Rights["Удалять файлы"]) && rights?.includes(Rights["Удаление классификаций"]) &&
                <div className={"file-controls__modal-item file-controls__modal-item_delete"}
                     onClick={() => dispatch(openModal({
                         id, content: ModalContent.Remove, callbackAccept: () => fetchFiles()
                     }))}>
                    <Delete/><span>Удалить</span></div>
                }
            </section>
            }
        </div>
    </OutsideAlerter>
});

type PropsType = { file: TypeFile, rights: Rights[], types: undefined | ExpandingObject<string>, fetchFiles: (...args: any) => void };
type ControlsPropsType = {
    id: string, dispatch: Dispatch<any>, rights: Rights[], fileType: string, fetchFiles: (...args: any) => void,
    classification?: ClassificationType | null
};

export default FragmentFile;


