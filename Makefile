CC = gcc
CFLAGS = -Wall -Werror -g

ifdef NO_OPT
CFLAGS += -O0
else
CFLAGS += -O2
endif

OBJS = jonesforth.o

all : jonesforth

jonesforth : $(OBJS)
	$(CC) $(CFLAGS) -o $@ $(OBJS)

clean :
	$(RM) jonesforth $(OBJS)
