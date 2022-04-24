import React, {ChangeEvent, FC, useState} from 'react';
import classes from "./../DocsClassesModal/DocsClassesModal.module.scss";
import {InputText} from "../../../utils/Inputs/Text/InputText";
import {Button} from "../../../utils/Button/Button";
import {ClassificationType} from "../../../../models/Classification";
import {useAppDispatch} from "../../../../utils/hooks/reduxHooks";
import {addClassification} from "../../../../redux/classesDocs/classesDocsThunks";

type Props = {
    onOutsideClick?: () => void
}

const Create: FC<Props> = ({onOutsideClick}) => {
    const dispatch = useAppDispatch();
    const [array, setArray] = useState<{ value: string }[]>([]);
    const [tag, setTag] = useState<string>("");
    const [name, setName] = useState<string>("");

    function saveTag() {
        if (tag.length > 0)
            setArray([...array, {value: tag}])
    }

    function onChangeTag(e: ChangeEvent<HTMLInputElement>) {
        setTag(e.target.value);
    }

    function onChangeName(e: ChangeEvent<HTMLInputElement>) {
        setName(e.target.value);
    }

    function onSubmit(){
        if (name.length === 0)
            return;
        dispatch(addClassification({name: name, classificationWords: array}));
    }

    return (
        <div className={classes.wrapper}>
            <h2 className={classes.h2}>Создание классификации документов</h2>
            <div className={classes.filters}>
                <div className={classes.filter}>
                    <label className={classes.label}>Название</label>
                    <InputText placeholder={"Введите название классификации"} onChange={onChangeName} value={name}/>
                </div>
                <div className={[classes.filter, classes.filter_class].join(" ")}>
                    <label className={classes.label}>Слова для классификации</label>
                    <div>
                        <InputText placeholder={"Введите слово для классификации"} onChange={onChangeTag} value={tag}/>
                        <Button onClick={saveTag}>Add</Button>
                    </div>
                    <div className={classes.tags}>
                        {array.map(e => {
                            return <div key={e.value} className={classes.tag}><span>{e.value}</span></div>
                        })}
                        <button className={classes.tagsBtn}>Показать все</button>
                    </div>
                </div>
            </div>

            <div className={classes.btns}>
                <Button type={"transparent"} onClick={onOutsideClick}>ОТМЕНА</Button>
                <Button onClick={onSubmit} disabled={name.length === 0}>ОК</Button>
            </div>
        </div>
    );
}

export default Create;
