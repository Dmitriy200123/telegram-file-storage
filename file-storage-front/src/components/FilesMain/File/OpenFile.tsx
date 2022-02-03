import React, {memo, useEffect} from 'react';
import "./File.scss"
import {ReactComponent as Svg} from "../../../assets/download.svg";
import {useAppDispatch, useAppSelector} from "../../../utils/hooks/reduxHooks";
import {ExpandingObject, ModalContent, Rights, TypeFile} from "../../../models/File";
import {Link} from 'react-router-dom';
import {fetchDownloadLink, fetchFile, fetchFileText} from "../../../redux/thunks/fileThunks";
import {ReactComponent as Edit} from "../../../assets/edit.svg";
import {ReactComponent as Delete} from "../../../assets/delete.svg";
import {filesSlice} from "../../../redux/filesSlice";
import {Button} from "../../utils/Button/Button";

const {openModal} = filesSlice.actions;
export const OpenedFileContainer: React.FC<any> = memo(({match}) => {
    const dispatch = useAppDispatch();
    
    const id = match.params["id"];
    const rights = useAppSelector((state) => state.profile.rights);
    const file = useAppSelector((state) => state.filesReducer.openFile);
    const filesTypes = useAppSelector((state) => state.filesReducer.filesTypes);

    useEffect(() => {
        if (file && id === file?.fileId) return;
        dispatch(fetchFile(id));
    }, [id])

    useEffect(() => {
        if (file && (+fileType === 4 || +fileType === 5))
            dispatch(fetchFileText({id, type: +fileType}))
    }, [file])

    if (!file)
        return null;

    const {fileType} = file;
    return <OpenedFile file={file} filesTypes={filesTypes || {}} id={id} rights={rights || []}/>;
})

const OpenedFile: React.FC<PropsType> = memo(({id, file, rights, filesTypes}) => {
    const dispatch = useAppDispatch();
    const {fileName, fileType, sender, chat, uploadDate, message} = file;
    const canRename = rights?.includes(Rights["Переименовывать файлы"]);

    function openRename() {
        dispatch(openModal({id: id, content: ModalContent.Edit}));
    }

    return (
        <div className={"file"}>
            <div className="file__header">
                <h2 className="file__title">Файл</h2>
                <Link className="file__close" to={"/files"}/>
            </div>
            <div className="file__content">
                <h3 className="file__content-title"
                    onClick={canRename ? openRename : undefined}>{fileName} {canRename && <Edit/>}</h3>
                <div className="file__item"><span>Формат: </span>{filesTypes && filesTypes[fileType]}</div>
                <div className="file__item"><span>Отправитель: </span><a>{sender?.fullName}</a></div>
                <div className="file__item"><span>Чат: </span><a>{chat?.name}</a></div>
                <div className="file__item"><span>Дата отправки: </span>{uploadDate}</div>
                {message && <div className="file__item">
                    <span>Сообщение: </span>{+fileType === 4 ? <a href={message}>{message} </a> : message}
                </div>}
                <div className={"file__btns"}>
                    {+fileType !== 5 && +fileType !== 4 && <button className="file__btn" onClick={() => {
                        dispatch(fetchDownloadLink(id))
                    }}>
                        <div>Скачать файл</div>
                        <Svg/>
                    </button>}
                    {rights?.includes(Rights["Удалять файлы"]) &&
                    <Button onClick={() => dispatch(() => dispatch(openModal({id: id, content: ModalContent.Remove})))}
                            type={"danger"} className={"file__btn_delete"}><span>Удалить</span><Delete/></Button>}
                </div>
            </div>
        </div>
    );
});

type PropsType = { id: string, file: TypeFile & { message?: string }, rights: Rights[], filesTypes: ExpandingObject<string> };




