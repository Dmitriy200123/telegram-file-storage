import React, {memo} from "react";
import "./LoadText.scss"
import {Select} from "../../utils/Inputs/Select";
import {SubmitHandler, useForm} from "react-hook-form";
import {Button} from "../../utils/Button/Button";
import {Link} from "react-router-dom";
import {InputText} from "../../utils/Inputs/Text/InputText";
import {useAppDispatch, useAppSelector} from "../../../utils/hooks/reduxHooks";
import {postCustomFile} from "../../../redux/thunks/fileThunks";
import {AppDispatch} from "../../../redux/redux-store";
import {useDispatch} from "react-redux";

export const LoadText: React.FC<{ dispatch: AppDispatch, className?: string }> = memo(({className}) => {
    const {
        handleSubmit,
        formState: {errors},
        setValue,
        getValues,
        register,
        reset
    } = useForm<FormType>();
    const dispatch = useAppDispatch();

    const {filesTypes} = useAppSelector(state => state.filesReducer);
    const optionsCategory = filesTypes && Object.keys(filesTypes).filter(key => key === "4" || key === "5")
        .map((key) => ({label: filesTypes[key], value: key}));

    const dispatchValuesForm: SubmitHandler<FormType> = ({contentType, FileName, message}) => {
        if (contentType && FileName && message) {
            dispatch(postCustomFile({contentType, FileName, message}));
            reset({contentType: null, FileName: null, message: null});
        }
    };


    const onChangeForm = handleSubmit(dispatchValuesForm);
    const setValueForm = (name: any, value: any) => {
        setValue(name, value, {
            shouldValidate: true,
            shouldDirty: true
        });
    }


    return (
        <div className={className + "  load-text"}>
            <form onSubmit={onChangeForm} className={"load-text__form"}>
                <div className={"load-text__form-inputes"}>
                    <div>
                        <p>Тип Файла</p>
                        <Select name={"contentType"}
                                setValue={setValueForm}
                                values={getValues("contentType")} options={optionsCategory} isMulti={false}
                                placeholder={""}/>
                    </div>
                    <label>
                        <p>Название</p>
                        <InputText type={"text"} form={{...register("FileName", {required: true})}}/>
                    </label>
                    <label>
                        <p>Ссылка или текст</p>
                        <InputText type={"text"} form={{...register("message", {required: true})}}/>
                    </label>
                </div>
                <div className={"load-text__btns"}>
                    <Button>Загрузить</Button>
                    <Link to={"/load/"}><Button type={"transparent"}>Отмена</Button></Link>
                </div>
            </form>
        </div>

    );
})

type FormType = { contentType: string | null, FileName: string | null, message: string | null };
