import React, {memo, useEffect} from 'react';
import "./File.scss"
import {ReactComponent as Svg} from "../../assets/download.svg";
import {useAppSelector} from "../../utils/hooks/reduxHooks";
import {Category, ModalContent} from "../../models/File";
import {Link} from 'react-router-dom';
import {useDispatch} from "react-redux";
import {fetchDownloadLink, fetchFile} from "../../redux/fileThunks";
import {ReactComponent as Edit} from "./../../assets/edit.svg";
import {ReactComponent as Delete} from "./../../assets/delete.svg";
import {filesSlice} from "../../redux/filesSlice";
import {Button} from "../utils/Button/Button";

const {openModal} = filesSlice.actions;
export const OpenedFile: React.FC<any> = memo(({match}) => {
    const id = match.params["id"];
    const file = useAppSelector((state) => state.filesReducer.openFile);
    const dispatch = useDispatch();
    useEffect(() => {
        if (file && id === fileId)
            return;
        dispatch(fetchFile(id));
    }, [id])
    if (!file)
        return null;
    const {fileName, fileId, fileType, sender, chat, uploadDate} = file;

    return (
        <div className={"file"}>
            <div className="file__header">
                <h2 className="file__title">Файл</h2>
                <Link className="file__close" to={"/files"}/>
            </div>
            <div className="file__content">
                <h3 className="file__content-title"
                    onClick={() => dispatch(openModal({id: id, content: ModalContent.Edit}))}>{fileName}<Edit/></h3>
                <div className="file__item"><span>Формат: </span>{Category[fileType]}</div>
                <div className="file__item"><span>Отправитель: </span><a>{sender.fullName}</a></div>
                <div className="file__item"><span>Чат: </span><a>{chat.name}</a></div>
                <div className="file__item"><span>Дата отправки: </span>{uploadDate}</div>
                <div className={"file__btns"}>
                    <button className="file__btn" onClick={() => {
                        dispatch(fetchDownloadLink(id))
                    }}>
                        <div>Скачать</div>
                        <Svg/>
                    </button>
                    <Button onClick={() => dispatch(() => dispatch(openModal({id: id, content: ModalContent.Remove})))} type={"danger"} className={"file__btn_delete"}><span>Удалить</span><Delete/></Button>
                </div>
            </div>
        </div>
    );
})




