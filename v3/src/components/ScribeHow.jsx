import clsx from 'clsx'

export function ScribeHow({ id, ...props }) {
    return <iframe src={`https://scribehow.com/embed/${id}`} width="640" height="640" allowFullScreen frameBorder="0" {...props} />;
}
