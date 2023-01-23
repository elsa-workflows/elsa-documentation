import clsx from 'clsx'

export function YouTube({ id, ...props }) {
    return <iframe width="560" height="315" 
                   src={`https://www.youtube.com/embed/${id}`} 
                   title="YouTube video player" frameBorder="0" 
                   allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share" 
                   allowFullScreen {...props} />;
}
