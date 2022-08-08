create table courses(
    id varchar(36) not null primary key,
    name varchar(255) not null,
    description text,
    status varchar(30),
    slug varchar(255),
    created_at timestamp not null
);

create table chapters(
    id varchar(36) not null,
    course_id varchar(36) not null,
    title varchar(255) not null,
    description text,
    media_id varchar(36) null,
    primary key(id, course_id),
    foreign key (course_id) references course(id),
    foreign key (media_id) references media(id)
);