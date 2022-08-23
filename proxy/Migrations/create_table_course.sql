create table courses(
    id varchar(36) not null primary key,
    name varchar(255) not null,
  	thumbnail varchar(200) null,
    fullimage varchar(200) null,
    description text,
    status varchar(30),
    slug varchar(255),
    created_at timestamp not null
);

create table modules(
    id varchar(36) not null,
    thumbnail varchar(200) null,
    fullimage varchar(200) null,
    course_id varchar(36) not null,
    title varchar(255) not null,
    description text,
    media_id varchar(36) null,
    primary key(id, course_id),
    foreign key (course_id) references courses(id),
    foreign key (media_id) references media(id)
);