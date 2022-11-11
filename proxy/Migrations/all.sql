create table media
(
    id         varchar(36)  not null
        primary key,
    name       varchar(255) not null,
    filename   varchar(255) not null,
    status     varchar(40),
    slug       varchar(255),
    created_at timestamp    not null
);

alter table media
    owner to postgres;

create table courses
(
    id          varchar(36)  not null
        primary key,
    name        varchar(255) not null,
    thumbnail   text,
    fullimage   text,
    description text,
    status      varchar(30),
    slug        varchar(255),
    created_at  timestamp    not null
);

alter table courses
    owner to postgres;

create table modules
(
    id          varchar(36)  not null,
    thumbnail   varchar(200),
    fullimage   varchar(200),
    course_id   varchar(36)  not null
        references courses,
    title       varchar(255) not null,
    description text,
    media_id    varchar(36)
        references media,
    primary key (id, course_id)
);

alter table modules
    owner to postgres;

create table users
(
    id         varchar(36)  not null
        primary key,
    name       varchar(200) not null,
    email      varchar(200) not null,
    password   varchar(255) not null,
    phone      varchar(30),
    user_type  varchar(20),
    created_at timestamp default CURRENT_TIMESTAMP
);

alter table users
    owner to postgres;

create table user_courses
(
    user_id   varchar(36) not null
        references users,
    course_id varchar(36) not null
        references courses,
    create_at timestamp default CURRENT_TIMESTAMP,
    primary key (user_id, course_id)
);

create table course_prices
(
    id         varchar(36) not null,
    course_id  varchar(36) not null
        constraint fk_courses
            references courses,
    created_at timestamp   not null,
    amount     numeric(8, 2),
    active     int not null default 1,
    constraint pk
        primary key (id, course_id)
);


alter table user_courses
    owner to postgres;

