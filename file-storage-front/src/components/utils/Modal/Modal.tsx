import React from 'react';
import "./Modal.scss"
import {Button} from "../Button/Button";
import Modal from "./ModalConfirm";
import {useDispatch} from "react-redux";
import {filesSlice} from "../../../redux/filesSlice";
import {fetchRemoveFile} from "../../../redux/fileThunks";

const {closeModal} = filesSlice.actions;

export const ModalConfirm: React.FC<{id:string}> = ({id}) => {
    // if (id === null)
    //     return null;
    const dispatch = useDispatch();
    return (
        <Modal>
            <div className={"modal-confirm"}>
                <h2 className={"modal-confirm__h2"}>Подтверждение</h2>
                <p className={"modal-confirm__p"}>Вы действительно хотите удалить это файл?</p>
                <div className={"modal-confirm__btns"}>
                    <Button onClick={() => dispatch(fetchRemoveFile(id))}>Да</Button>
                    <Button onClick={() => dispatch(closeModal())} type={"transparent"} >Нет</Button>
                </div>
                {id}
            </div>
        </Modal>
    )
}

